
import * as THREE from 'three';
import { GLTFLoader } from 'https://unpkg.com/three@0.163.0/examples/jsm/loaders/GLTFLoader.js';
import { OrbitControls } from 'https://unpkg.com/three@0.163.0/examples/jsm/controls/OrbitControls.js';
const container = document.getElementById('container');
class Game {
    scene;
    raycaster;
    clock;
    mouse;
    camera;
    renderer;
    constrols;
    board;
    id;
    started = false;
    tableId = localStorage.getItem('tableId');
    currentPlayer;

    constructor(playercount){
        this.board = new Board(playercount + 3);

        //Initialize game world
        this.scene = new THREE.Scene();
        this.raycaster = new THREE.Raycaster();
        this.raycaster.params.Points.threshold = 0.1; 
        this.raycaster.params.Line.threshold = 0.1; 
        this.clock = new THREE.Clock();
        this.mouse = new THREE.Vector2();
        this.scene.background = new THREE.Color(0x87ceeb);
        this.scene.fog = new THREE.Fog( 0xcccccc, 15, 25 );
        this.containerWidth = container.clientWidth;
        this.containerHeight = container.clientHeight;
        this.camera = new THREE.PerspectiveCamera(75, this.containerWidth / this.containerHeight, 0.1, 1000);
        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        const onWindowResize = () => {
            // Update container dimensions
            const containerWidth = container.clientWidth;
            const containerHeight = container.clientHeight;
            // Update camera aspect ratio
            this.camera.aspect = containerWidth / containerHeight;
            this.camera.updateProjectionMatrix();
            // Update renderer size
            this.renderer.setSize(containerWidth, containerHeight);
            this.camera.fov = 60;
            this.camera.updateProjectionMatrix();
        };
        // Add event listener for window resize
        window.addEventListener('resize', onWindowResize);
        // Call onWindowResize initially to set up the correct dimensions
        onWindowResize();
        this.pixelRatio = window.devicePixelRatio || 1; // Get device pixel ratio
        this.renderer.setPixelRatio(this.pixelRatio);
        this.renderer.setSize(this.containerWidth, this.containerHeight, false);
        this.ambientLight = new THREE.AmbientLight(0xffffff, 0.7);
        this.scene.add(this.ambientLight);
        this.sunColor = 0xF7EACD; // A warm, yellowish-orange color (Like the sun)
        this.sunLight = new THREE.PointLight(this.sunColor, 60, 0, 1);
        this.sunLight.position.set(10, 10, 0);
        this.scene.add(this.sunLight);
        container.appendChild(this.renderer.domElement);

        //Orbit Controls
        this.controls = new OrbitControls(  
            this.camera, 
            this.renderer.domElement
        );
        this.controls.enableDamping = true; 
        this.controls.dampingFactor = 0.2;
        this.controls.screenSpacePanning = false;
        this.controls.enablePan = false;
        this.controls.minDistance = 6;
        this.controls.maxDistance = 16;
        this.controls.maxPolarAngle = Math.PI / 2.25;
        this.controls.target.z = -0;

        //Load Gameboard, for now only 5x5
        this.loader = new GLTFLoader();
        const self = this;
        this.loader.load('assets/board.gltf', function (gltf) {
            self.boardObject = gltf.scene;
            self.boardObject.rotation.y = 1.5708;
            self.boardObject.position.y = -0.15;
            self.scene.add(gltf.scene);  

        }, undefined, function (error) {
            console.error(error);
        });

        this.camera.position.z = -12;
        this.camera.position.y = 4.5;
        this.camera.rotation.x = -0.5;

        animate();
    }

    fillModels() {
        const teams = {1: this.team1, 2: this.team2, 3: this.team3, 4: this.team4};
        for (let i = 0; i < this.board.currentBoard.length; i++) {
            for (let k = 0; k < this.board.currentBoard[i].length; k++) {
                const [id, type] = this.board.currentBoard[i][k];
                const teamId = Math.ceil(id / 5);
                const team = teams[teamId];
                this.placeObject(team, [i, k], id, type);
            }
        }
    }

    placeObject(team, coord, identity, type){
        console.log("placing object")
        if(identity == 0){
            console.log("Identity: 0")
            const material = new THREE.MeshBasicMaterial({
                color: 0xffffff, //White
                transparent: true, // Transparent
                opacity: 0.1, // Opacity to 0.1
            });

            const cubeGeometry = new THREE.BoxGeometry(1.25, 0.05, 1.25);

            // Create a mesh using the geometry and material
            const cube = new THREE.Mesh(cubeGeometry, material);
            cube.name = "emptyslot"+identity;
            cube.position.x = coord[1] * 2 - 4;
            cube.position.z = coord[0] * 2 - 4;
            this.scene.add(cube);
            return;
        }

        const colorMap = {
            "Green": { boxColor: 0x13F287, ninja: 'assets/green_ninja.gltf', master: 'assets/green_master.gltf' },
            "Red": { boxColor: 0xff0000, ninja: 'assets/red_ninja.gltf', master: 'assets/red_master.gltf' },
            "Yellow": { boxColor: 0xfffb0c, ninja: 'assets/yellow_ninja.gltf', master: 'assets/yellow_master.gltf' },
            "Orange": { boxColor: 0xff5c0c, ninja: 'assets/orange_ninja.gltf', master: 'assets/orange_master.gltf' },
            "Blue": { boxColor: 0x0c82ff, ninja: 'assets/blue_ninja.gltf', master: 'assets/blue_master.gltf' }
        };
        const { boxColor, ninja, master } = colorMap[team.color];
        const asset = type === "k" ? master : ninja;
        const orient = type === "k" ? team.facing + 2 : team.facing;
        const tScale = type === "k" ? 1 : 2;

        const self = this;
        this.loader.load(asset, function (gltf) {
            const modelObject = gltf.scene;
            const mixer = new THREE.AnimationMixer(modelObject);
            const action = mixer.clipAction( gltf.animations[0] );
            self.scene.add(gltf.scene);
            modelObject.rotation.y = orient * 1.57 * 2;
            modelObject.position.z = coord[0] * 2 - 4;
            modelObject.position.x = coord[1] * 2 - 4;
            action.timeScale = tScale;
            action.play(); 

            const material = new THREE.MeshBasicMaterial({
                color: boxColor,
                transparent: true,
                opacity: 0.01,
            });

            // Create selection (flat) cube
            const cubeGeometry = new THREE.BoxGeometry(1.25, 0.05, 1.25);
            const cube = new THREE.Mesh(cubeGeometry, material);
            cube.name = team.number + "hover"+identity;
            modelObject.add(cube);
            self.board.currentBoard[coord[0]][coord[1]] = [identity, type, modelObject, mixer, gltf];
        }, undefined, function (error) {
            console.error(error);
        });
    }

    start(){
        game.started = true;
        container.classList.remove('container-waiting');
        document.querySelector('.loading').remove();
        document.querySelector('.game-button-start').classList.add('game-button-hidden');
    }
};

class Board {
    constructor(size){
        switch (size) {
            case 7:
                this.currentBoard = [
                    [[0], [1, "p"], [2, "p"], [3, "k"], [4, "p"], [5, "p"], [0]],
                    [[11, "p"], [0], [0], [0], [0], [0], [16, "p"]],
                    [[12, "p"], [0], [0], [0], [0], [0], [17, "p"]],
                    [[13, "k"], [0], [0], [0], [0], [0], [18, "p"]],
                    [[14, "p"], [0], [0], [0], [0], [0], [19, "p"]],
                    [[15, "p"], [0], [0], [0], [0], [0], [20, "p"]],
                    [[0], [6, "p"], [7, "p"], [8, "k"], [9, "p"], [10, "p"], [0]],
                ];
                break;
        
            default:
                this.currentBoard = [
                    [[1, "p"], [2, "p"], [3, "k"], [4, "p"], [5, "p"]],
                    [[0], [0], [0], [0], [0]],
                    [[0], [0], [0], [0], [0]],
                    [[0], [0], [0], [0], [0]],
                    [[6, "p"], [7, "p"], [8, "k"], [9, "p"], [10, "p"]],
                ];
                break;
        }
    }
}

var game = new Game(2);
window.game = game;

const loadingDivElement = document.createElement('div');
loadingDivElement.className = 'loading';
container.appendChild(loadingDivElement);

function giveAnimation(type) {
    //This needs to be changed to individual models
    const board = game.board;
    for (let i = 0; i < board.currentBoard.length; i++) {
        for (let k = 0; k < board.currentBoard.length; k++) {
            const el = board.currentBoard[i][k][3];
            if(board.currentBoard[i][k][0] == 0){

            } else if (el != undefined) {
                try {
                    let mixer = board.currentBoard[i][k][3];
                    let action;
                    if (type == "jump"){
                        action = mixer.clipAction( board.currentBoard[i][k][4].animations[1] );
                    } else {
                        action = mixer.clipAction( board.currentBoard[i][k][4].animations[3] );
                    }
                    
                    mixer.stopAllAction();
                    action.play(); 
                } catch {
                    
                }
                
            }
            
        }
    }
}

function animate() {
    requestAnimationFrame(animate);
    if(game == undefined){
        return;
    }
    const clock = game.clock;
    const board = game.board;
    const controls = game.controls;
    const renderer = game.renderer;
    let delta = clock.getDelta();
    for (let i = 0; i < board.currentBoard.length; i++) {
        for (let k = 0; k < board.currentBoard.length; k++) {
            const el = board.currentBoard[i][k][3];
            if(board.currentBoard[i][k][0] == 0){

            } else if (el != undefined) {
                el.update( delta );
            }
        }
    }
    controls.update();
    renderer.render(game.scene, game.camera);
}

function onPointerMove(event) {
    if (game === undefined) {
        return;
    }
    // Get the pointer coordinates in screen space
    let pointerX, pointerY;
    if (event.type.startsWith('touch')) {
        // For touch events, get the coordinates of the first touch point
        pointerX = event.touches[0].clientX;
        pointerY = event.touches[0].clientY;
    } else {
        // For mouse events, use clientX and clientY directly
        pointerX = event.clientX;
        pointerY = event.clientY;
    }
    // Set the mouse coordinates in the game object
    game.mouseX = pointerX;
    game.mouseY = pointerY;
    // Get the bounding rectangle of the renderer element
    let rect = game.renderer.domElement.getBoundingClientRect();
    // Calculate the mouse coordinates relative to the renderer element
    let x = (game.mouseX - rect.left) / rect.width * 2 - 1;
    let y = -(game.mouseY - rect.top) / rect.height * 2 + 1;
    // Update the mouse vector
    game.mouse.set(x, y);
    // update the picking ray with the camera and mouse position
    game.raycaster.setFromCamera(game.mouse, game.camera);
    // Reset previously highlighted objects
    resetHighlightedObjects();
    // calculate objects intersecting the picking ray
    let intersects = game.raycaster.intersectObjects(game.scene.children);
    let cubes = 0;
    if (intersects.length > 0) {
        intersects.forEach(element => {
            let object = element.object;
            if(object.name.includes(game.currentPlayer+"hover")){
                object.material.opacity = 0.8;
                cubes++;
            }
        });
    }
}

function resetHighlightedObjects() {
    // Reset opacity for all objects with the name "hover"
    game.scene.traverse(function(child) {
        if (child instanceof THREE.Mesh && child.name.includes("hover")) {
            child.material.opacity = 0.3; // Reset opacity to default
            child.material.needsUpdate = true; // Update material
        }
    });
}

// Add event listeners for pointer and touch events
window.addEventListener('pointermove', onPointerMove, false);
window.addEventListener('touchmove', onPointerMove, false);
window.addEventListener('touchstart', onPointerMove, false);
window.addEventListener('touchend', resetHighlightedObjects, false);

async function fetchTable(){
    if(game.started == true){
        return;
    }
    const response = await fetch(currentApi + "/api/Tables/" + game.tableId, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, '500', "#c60025");
            });
        }
        return response.json();
    }).then(data => {
        //console.log(data);

        const table = data;
        if(game.currentPlayer == undefined){
            //console.log("Not undefined");
            const className = "container-"+table.preferences.numberOfPlayers+"-"+table.seatedPlayers.length;
            console.log(className);
            container.classList.add(className);
            if(data.hasAvailableSeat != true){
                //The table is full, do stuff
                container.classList.remove('container-loading');
                container.classList.add('container-waiting');
                //Set team1, 2, (3 and 4) as players in order, or by direction.
                for (let i = 0; i < data.seatedPlayers.length; i++) {
                    const player = data.seatedPlayers[i];
                    if(player.direction == "North"){
                        player.facing = 1;
                        player.number = 1;
                        game.team1 = player;
                        if(user.id == player.id){
                            game.currentPlayer = 1;
                        }
                    } else if (player.direction == "South"){
                        player.facing = 0;
                        player.number = 2;
                        game.team2 = player;
                        if(user.id == player.id){
                            game.currentPlayer = 2;
                            game.camera.position.set(0, 4.5, 12); // Move the camera along the negative z-axis
                            game.camera.lookAt(0, 0, 0); // Look back towards the origin

                            // Update OrbitControls to reflect changes
                            game.controls.update();

                        }
                    }
                }
                //Orient game camera so that the player sees his team in front
                if(game.currentPlayer == 1){
                    //Orient camera
                } else if (game.currentPlayer == 2){
                    //Orient camera for player 2
                }
                //Maybe set limits for cam rotation?
                //fill the models based on team color
                console.log("filling models");
                game.fillModels();
                //Make sure the current player can only control his own pawns, and only when it is his turn.
                //Move this to its own function
            }

        setTimeout(fetchTable, 500);
        } else if(game.id == undefined || game.id == "00000000-0000-0000-0000-000000000000") {
            game.id = table.gameId;
            console.log(game.id);
            setTimeout(fetchTable, 500);
        } else {
            game.start();
            console.log(game.id);
        }
        //setTimeout(fetchTable, 2500);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
}
window.fetchTable = fetchTable;
