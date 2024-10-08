
import * as THREE from 'three';
import { GLTFLoader } from 'https://unpkg.com/three@0.163.0/examples/jsm/loaders/GLTFLoader.js';
import { OrbitControls } from 'https://unpkg.com/three@0.163.0/examples/jsm/controls/OrbitControls.js';
const container = document.getElementById('container');
const gameId = localStorage.getItem("gameId");
let previousBoard;
let perspective = 1;
window.perspective = perspective;
let hoveredCube;
let clickedCube;
let openCubes = [];
window.openCubes = openCubes;
let hovering = false;
let lastPointerCoords = { x: 0, y: 0 };
window.hoveredCube = hoveredCube;
window.hovering = hovering;
window.clickedCube = clickedCube;
let allUsers;
let ownerUser;
let selectedPawn;
window.selectedPawn = selectedPawn;
let pawnCoords;
let selectionCoords;
let allCubes = [];
window.allCubes = allCubes;
let tableFetchInterval;
let getGameInterval;

async function fetchSpiritMoves() {
    try {
        const response = await fetch('../files/cardSets/way-of-the-wind.json');
        const json = await response.json();
        return json;
    } catch (error) {
        console.error('Error fetching JSON:', error);
    }
}

class Game {
    scene; //The 3D scene for ThreeJS. All of our 3D models will go inside here
    raycaster; //Used for mapping mouse click positions to 3D objects in the scene
    clock;
    mouse;
    camera;
    renderer; //WebGL renderer
    controls; //Orbit Controls
    board; //3D game board model
    id; //Game ID
    started = false; //Did the game start?
    loaded = false; //Did the game load?
    tableId = localStorage.getItem('tableId');
    currentPlayer; //Integer value (1 or 2)
    playerToPlay; //Whose turn it is
    playerCards; //Cards of the current player
    selectedCard; //Selected card name
    selectedMove; //In case of WOTW, the "normal pawn" move selected
    selectedMoveSpirit; //In case of WOTW, the "spirit" move selected
    spiritMoves; //All the spirit moves loaded from a JSON
    currentPlayerObject; //JS object with JSON data of the player
    gameType; //Type of the game (classic, competitive, blitz, way of the wind, etc)
    oldElo = user.elo; //The "Previous" ELO of the player. Used to check with updated value and compare
    lastClicked; //The object which the user last clicked

    constructor(playercount){
        this.board = new Board(playercount + 3);

        //Initialize game world
        this.scene = new THREE.Scene();
        this.raycaster = new THREE.Raycaster();
        this.raycaster.params.Points.threshold = 0.1; 
        this.raycaster.params.Line.threshold = 0.1; 
        this.clock = new THREE.Clock();
        this.mouse = new THREE.Vector2();
        this.containerWidth = container.clientWidth;
        this.containerHeight = container.clientHeight;
        this.camera = new THREE.PerspectiveCamera(75, this.containerWidth / this.containerHeight, 0.1, 1000);
        this.renderer = new THREE.WebGLRenderer({ antialias: true }); //Uses antialiasing to make the game prettier
        this.renderer.shadowMap.enabled = true; //Shadows!
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap; //Soft shadows!
        const onWindowResize = () => { //Fix aspect ratio when device screen resizes
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
        this.boardAsset = "";

        if(userSettings["toggle-moon"] === 'true'){ //Moon theme
            this.boardAsset = "assets/board-space.gltf";
            const loader = new THREE.CubeTextureLoader();
            const texture = loader.load([
            'assets/textures/sky-space/bkg1_right.webp', // positive X
            'assets/textures/sky-space/bkg1_left.webp', // negative X
            'assets/textures/sky-space/bkg1_top.webp', // positive Y
            'assets/textures/sky-space/bkg1_bot.webp', // negative Y
            'assets/textures/sky-space/bkg1_front.webp', // positive Z
            'assets/textures/sky-space/bkg1_back.webp'  // negative Z
            ]); //Skybox sourced from https://opengameart.org/content/space-skyboxes-0 with author's permission
            this.scene.background = texture;
            this.ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
            this.sunColor = 0xB5ACE1; // A warm, yellowish-orange color (Like the sun)
            this.sunLight = new THREE.DirectionalLight(this.sunColor, 3);
        } else if(userSettings["toggle-aqua"] === "true"){ //Aqua theme
            this.boardAsset = "assets/board-water.gltf";
            const loader = new THREE.CubeTextureLoader();
            const texture = loader.load([
            'assets/textures/sky-water/px.webp', // positive X
            'assets/textures/sky-water/nx.webp', // negative X
            'assets/textures/sky-water/py.webp', // positive Y
            'assets/textures/sky-water/ny.webp', // negative Y
            'assets/textures/sky-water/pz.webp', // positive Z
            'assets/textures/sky-water/nz.webp'  // negative Z
            ]); //Skybox sourced from https://www.deviantart.com/kuby64/art/My-second-underwater-HDRI-916960638 with Author's permission
            this.scene.background = texture;
            this.ambientLight = new THREE.AmbientLight(0xffffff, 1.3);
            this.sunColor = 0xF7EACD; // More blueish (underwater)
            this.sunLight = new THREE.DirectionalLight(this.sunColor, 5);
        } else if(userSettings["toggle-sahara"] === "true"){ //Sahara theme
            this.boardAsset = "assets/board-desert.gltf";
            const loader = new THREE.CubeTextureLoader();
            const texture = loader.load([
            'assets/textures/sky-desert/px.webp', // positive X
            'assets/textures/sky-desert/nx.webp', // negative X
            'assets/textures/sky-desert/py.webp', // positive Y
            'assets/textures/sky-desert/ny.webp', // negative Y
            'assets/textures/sky-desert/pz.webp', // positive Z
            'assets/textures/sky-desert/nz.webp'  // negative Z
            ]); //Skybox sourced from https://hdri-haven.com/hdri/desert-nature-reserve (CC0 license)
            this.scene.background = texture;
            this.ambientLight = new THREE.AmbientLight(0xffffff, 1.3);
            this.sunColor = 0xFFDD9D; // Even warmer than the default (Desert feel)
            this.sunLight = new THREE.DirectionalLight(this.sunColor, 6);
        } else { //Classic theme
            this.boardAsset = "assets/board.gltf";
            this.scene.background = new THREE.Color(0x87ceeb);
            this.ambientLight = new THREE.AmbientLight(0xffffff, 1.2);
            this.sunColor = 0xF7EACD; // A warm, yellowish-orange color (Like the sun)
            this.sunLight = new THREE.DirectionalLight(this.sunColor, 5);
        }

        this.scene.add(this.ambientLight);
        
        this.sunLight.position.set(14, 14, 0);
        this.sunLight.target.position.set(0, 0, 10);
        this.scene.add( this.sunLight.target ); 
        this.sunLight.castShadow = true;
        this.scene.add(this.sunLight);
        //Set up shadow properties for the light
        this.sunLight.shadow.camera.top = 20;
        this.sunLight.shadow.camera.bottom = - 20;
        this.sunLight.shadow.camera.left = - 20;
        this.sunLight.shadow.camera.right = 20;
        this.sunLight.shadow.bias = -0.005; //This fixes shadows looking weird
        this.sunLight.shadow.mapSize.width = 2048; // default
        this.sunLight.shadow.mapSize.height = 2048; // default
        this.sunLight.shadow.camera.near = 0.1; // default
        this.sunLight.shadow.camera.far = 50; // default
        //this.scene.add( new THREE.CameraHelper( this.sunLight.shadow.camera ) );
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
        this.loader.load(this.boardAsset, function (gltf) {
            self.boardObject = gltf.scene;
            self.boardObject.rotation.y = 1.5708;
            self.boardObject.position.y = -0.15;
            self.boardObject.receiveShadow = true;
            self.boardObject.castShadow = true;
            gltf.scene.traverse(function (child) {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                    if(userSettings["toggle-aqua"] === "true"){ //Aqua mode requires a special shader for caustics
                        const objectTexture = child.material.map;
                        const boundingBox = new THREE.Box3().setFromObject(child);
                        const size = new THREE.Vector3();
                        const resolution = new THREE.Vector2(container.clientWidth, container.clientHeight);
                        boundingBox.getSize(size);
                        const uvScale = new THREE.Vector2(0.3 * size.x, 0.3 * size.z);
                        child.material = new THREE.ShaderMaterial({
                            uniforms: {
                                time: { value: 0.0 },
                                resolution: { value: resolution },
                                mouse: { value: new THREE.Vector2() },
                                objectTexture: { value: objectTexture },
                                uvScale: { value: uvScale } // Add UV scale uniform
                            },
                            vertexShader: `
                                varying vec2 surfacePosition;
                        
                                void main() {
                                    surfacePosition = uv;
                                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
                                }
                            `,
                            fragmentShader: `
                                //Base shader code sourced from https://glslsandbox.com/e#54421.0
                                #ifdef GL_ES
                                precision mediump float;
                                #endif
                        
                                uniform float time;
                                uniform vec2 mouse;
                                uniform vec2 resolution;
                                uniform sampler2D objectTexture;
                                uniform vec2 uvScale;
                                varying vec2 surfacePosition;
                        
                                #define MAX_ITER 5 // water depth
                        
                                void main( void ) {
                                    vec2 sp = surfacePosition.xy * uvScale; // Apply UV scaling
                                    vec2 p = sp * 15.0 - vec2(27.0);
                                    vec2 i = p;
                                    float c = 0.5; // brightness; larger -> darker
                                    float inten = 0.06; // brightness; larger -> brighter
                                    float speed = 1.5; // larger -> slower
                                    float speed2 = 3.0; // larger -> slower
                                    float freq = 0.8; // ripples
                                    float xflow = 1.5; // flow speed in x direction
                                    float yflow = 0.0; // flow speed in y direction
                        
                                    for (int n = 0; n < MAX_ITER; n++) {
                                        float t = time * (1.0 - (3.0 / (float(n) + speed)));
                                        i = p + vec2(cos(t - i.x * freq) + sin(t + i.y * freq) + (time * xflow), sin(t - i.y * freq) + cos(t + i.x * freq) + (time * yflow));
                                        c += 1.0 / length(vec2(p.x / (sin(i.x + t * speed2) / inten), p.y / (cos(i.y + t * speed2) / inten)));
                                    }
                        
                                    c /= float(MAX_ITER);
                                    c = 1.5 - sqrt(c);
                                    vec4 causticsColor = vec4(vec3(c * c * c * c), 0.0) + vec4(0.0, 0.4, 0.55, 1.0);
                        
                                    vec4 objectColor = texture2D(objectTexture, surfacePosition);
                        
                                    // Blend caustics color with object texture
                                    gl_FragColor = objectColor + causticsColor * 0.2; // Adjust blending factor as needed
                                }
                            `
                        });
                    }
                }
            });
            self.scene.add(gltf.scene);  

        }, undefined, function (error) {
            console.error(error);
        });

        this.camera.position.z = -12;
        this.camera.position.y = 4.5;
        this.camera.rotation.x = -0.5;
        animate();
    }

    setCamera(){
        if(this.currentPlayer == 1){
            this.camera.position.z = -12;
            this.camera.position.y = 4.5;
            this.camera.rotation.x = -0.5;
            this.camera.lookAt(0, 0, 0);
        } else {
            this.camera.position.set(0, 4.5, 12); // Move the camera along the negative z-axis
            this.camera.lookAt(0, 0, 0); // Look back towards the origin
        }
    }

    showCoordinates(){
        console.log(this.board.currentBoard);
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
            gltf.scene.traverse(function (child) {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            if(userSettings["toggle-moon"] == 'true' || userSettings["toggle-aqua"] == "true"){
                //Load the space helmet!
                self.loader.load('assets/space-helmet.gltf', function (helmetgltf) {
                    console.log(gltf.scene);
                    helmetgltf.scene.traverse((node) => {
                        if (node.isMesh) {
                            node.material.transparent = true;
                            node.material.alphaTest = 0.3; // optional, adjust based on your needs
                            // node.material.opacity = 0.5; // optional, use if you want to change overall opacity
                            node.material.needsUpdate = true; // ensure the material updates
                        }
                    });
                    helmetgltf.scene.scale.set(1.3, 1.3, 1.3);
                    helmetgltf.scene.position.y -= 0.2;
                    gltf.scene.traverse(function (child) {
                        child.children.forEach(element => {
                            if(element.name == "head"){
                                element.add(helmetgltf.scene);
                                console.log(element.name);
                            }
                        });
                    });
                });
            }
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
            cube.onitamaType = "pawn";

            cube.scale.y = 1.01;
            cube.scale.x = 1.01;
            cube.scale.z = 1.01;
            modelObject.add(cube);
            self.board.currentBoard[coord[0]][coord[1]] = [identity, type, modelObject, mixer, gltf, cube];
        }, undefined, function (error) {
            console.error(error);
        });
    }
    rotate180(array) {
        // Reversing the rows
        let reversedRows = array.slice().reverse();
        
        // Reversing the elements in each row
        let rotatedArray = reversedRows.map(row => row.slice().reverse());
        
        return rotatedArray;
    }

    async getPossibleCoordinate(pawnId){
        let coordinates;
        await fetch(currentApi + "/api/Games/" + game.id + "/possible-moves-of/" + pawnId + "/for-card/" + this.selectedCard, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        }).then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw_floating_error(errorData.message, '405', "#c60025");
                });
            }
            return response.json();
        }).then(data => {
            console.log(data);
            if(data[0] != undefined){
                console.log("Available row " + data[0].to.row + " and col " + data[0].to.column);
            }
            coordinates = data;
        }).catch(error => {
            console.log(error);
            //throw_floating_error(error, '500', "#c60025");
        });

        return coordinates;
    }

    movePawn(id, cardName, coords){
        let x = coords[0];
        let y = coords[1];

        console.log(x);
        console.log(y);

        console.log(game.currentPlayerObject.direction)

        console.log(id);
        console.log(cardName);
        console.log(x); //row
        console.log(y); //col

        console.log("We are trying to move pawn to row " + x + " and col " + y);

        //Then try to do the move
        let action;
        if(game.gameType == "competitive"){
            action = "/move-pawn-competitive";
        } else {
            action = "/move-pawn-ai";
        }
        const response = fetch(currentApi + "/api/games/" + game.id + action, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }, 
            body: JSON.stringify({ pawnId: id, moveCardName: cardName, to: {row: x, column: y} })
        }).then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw_floating_error(errorData.message, '405', "#c60025");
                });
            }
            //return response.json();
        }).then(data => {
            //console.log(data);
        }).catch(error => {
            console.log(error);
            //throw_floating_error(error, '500', "#c60025");
        });
    }

    movePawnSpirit(){ //Does 2 pawn moves, in case the game is WOTW and the movecard is in the WOTW cardset
        const action = "/move-pawn-wotw";
        const pawnId = game.selectedMove[0];
        const cardName = game.selectedMove[1];
        const x = game.selectedMove[2][0];
        const y = game.selectedMove[2][1];
        const spiritId = game.selectedMoveSpirit[0];
        const xSpirit = game.selectedMoveSpirit[2][0];
        const ySpirit = game.selectedMoveSpirit[2][1];

        const response = fetch(currentApi + "/api/games/" + game.id + action, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }, 
            body: JSON.stringify({ pawnId: pawnId, moveCardName: cardName, to: {row: x, column: y}, SpiritId: spiritId, spiritTo: {row: xSpirit, column: ySpirit} })
        }).then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw_floating_error(errorData.message, '405', "#c60025");
                });
            }
            //return response.json();
        }).then(data => {
            console.log(data);
            //Clear the moves
            game.selectedMove = undefined;
            game.selectedMoveSpirit = undefined;
        }).catch(error => {
            console.log(error);
            //throw_floating_error(error, '500', "#c60025");
        });
    }

    loadSpirit(){ //Load's the spirit (in case the game is WOTW)
        let team;
        if(game.currentPlayer == 1){
            team = game.team1;
        } else {
            team = game.team2;
        }
        let coord = [2, 2];
        let identity = 200;
        let type = "s";
            console.log("placing object")
    
            const colorMap = {
                "Green": { boxColor: 0x13F287},
                "Red": { boxColor: 0xff0000},
                "Yellow": { boxColor: 0xfffb0c},
                "Orange": { boxColor: 0xff5c0c},
                "Blue": { boxColor: 0x0c82ff}
            };
            const { boxColor} = colorMap[team.color];
            const asset = 'assets/spirit.gltf';
    
            const self = this;
            this.loader.load(asset, function (gltf) {
                const modelObject = gltf.scene;
                const mixer = new THREE.AnimationMixer(modelObject);
                //const action = mixer.clipAction( gltf.animations[0] );
                gltf.scene.traverse(function (child) {
                    if (child.isMesh) {
                        child.castShadow = true;
                        child.receiveShadow = true;
                    }
                });
                self.scene.add(gltf.scene);
                //modelObject.rotation.y = orient * 1.57 * 2;
                modelObject.position.z = coord[0] * 2 - 4;
                modelObject.position.x = coord[1] * 2 - 4;
                //action.timeScale = tScale;
                //action.play(); 
    
                const material = new THREE.MeshBasicMaterial({
                    color: boxColor,
                    transparent: true,
                    opacity: 0.01,
                });
    
                // Create selection (flat) cube
                const cubeGeometry = new THREE.BoxGeometry(1.25, 0.05, 1.25);
                const cube = new THREE.Mesh(cubeGeometry, material);
                cube.name = team.number + "hover"+identity;
                cube.onitamaType = "pawn";
    
                cube.scale.y = 1.01;
                cube.scale.x = 1.01;
                cube.scale.z = 1.01;
                modelObject.add(cube);
                self.board.currentBoard[coord[0]][coord[1]] = [identity, type, modelObject, mixer, gltf, cube];
            }, undefined, function (error) {
                console.error(error);
            });
    }

    movePawn3D(change){ //The Visual change when a pawn moves
        let from = [change.from.row, 4 - change.from.column]; //Starting coordinates
        console.log(from);
        let to = [4 - change.to.column, change.to.row]; //Ending coordinates
        let pawnId = change.object.Id;
        console.log("POSITION")
        const board = this.board.currentBoard;
        for (let i = 0; i < board.length; i++) {
            for (let j = 0; j < board[i].length; j++) {
                const el = board[i][j];
                if(el[6] != undefined){
                    let theId = el[6].id;
                    if(el[6].id == undefined){
                        theId = el[6].Id;
                    }
                    if(theId == pawnId){
                        console.log("Pawn found at " + i + ", " + j);
                        console.log(el[2].position);
                        let x = ((to[0]) - 2) * 2;
                        let z = ((to[1]) - 2) * 2;
                        let mixer = el[3];
                        console.log(el[4]);
                        let action = mixer.clipAction(el[4].animations[1]);
                        mixer.timeScale = 1.4; //Speed up the jump animation. It's a little slow
                        action.setLoop(THREE.LoopOnce); //Only jump one time
                        action.clampWhenFinished = true;
                        action.setDuration(el[4].animations[1].duration);
                        action.play();
                        //Maybe do it a bit short of the position if we need to attack
                        setTimeout(game.smoothTransition.bind(null, el[2], x, 0, z, 650), 420); //Transition to new position
                        // Listen for when the animation finishes
                        mixer.addEventListener('finished', function(){
                            //If we need to do an attack, we should probably do it here.
                            let loopAction = mixer.clipAction(el[4].animations[0]);
                            // Set animation to loop
                            loopAction.setLoop(THREE.LoopRepeat);
                            // Start playing idle animation
                            loopAction.play();
                        });
                        //After the attack, we can move the remaining space
                        //Future dev (Teo) here: If we have time left, we can use attack animations here.
                        //Otherwise they might stay unused.
                    }
                }
                
            }
        }

    }

    removePawn3D(change){ //Removing a pawn from the board (Miving them down).
        let from = [change.from.row, 4 - change.from.column]; //Starting coordinates
        console.log(from);
        let pawnId = change.object.Id;
        console.log("REMOVED POSITION")
        const board = this.board.currentBoard;
        for (let i = 0; i < board.length; i++) {
            for (let j = 0; j < board[i].length; j++) {
                const el = board[i][j];
                if(el[1] != undefined){
                    if(el[6] == null || el[6].id == pawnId){
                        console.log("Removed Pawn found at " + i + ", " + j);
                        console.log(el[2].position);
                        let x = el[2].position.x;
                        let z = el[2].position.z;
                        let mixer = el[3];
                        console.log(el[4]);
                        let action = mixer.clipAction(el[4].animations[0]);
                        mixer.timeScale = 1.4;
                        action.setLoop(THREE.LoopOnce);
                        action.clampWhenFinished = true;
                        action.setDuration(el[4].animations[0].duration);
                        action.play();
                        setTimeout(game.smoothTransition.bind(null, el[2], x, -3, z, 650), 420); //Transition to new position
                        // Listen for when the animation finishes
                        mixer.addEventListener('finished', function(){
                            let loopAction = mixer.clipAction(el[4].animations[0]); //Idle animation
                            // Set animation to loop
                            loopAction.setLoop(THREE.LoopRepeat);
                            // Start playing idle animation
                            loopAction.play();
                        });
                        //After the attack, we can move the remaining space
                    }
                }
                
            }
        }

        console.log(board);

    }

    smoothTransition(object, endX, endY, endZ, duration) { //Helps MovePawn3D in actually moving the pawns
        const startX = object.position.x;
        const startY = object.position.y;
        const startZ = object.position.z;
        const startTime = performance.now();
    
        function updatePosition() {
            const elapsed = performance.now() - startTime;
            const progress = elapsed / duration;
    
            if (progress >= 1) {
                object.position.x = endX;
                object.position.y = endY;
                object.position.z = endZ;
                return;
            }
    
            object.position.x = game.lerp(startX, endX, progress);
            object.position.y = game.lerp(startY, endY, progress);
            object.position.z = game.lerp(startZ, endZ, progress);
    
            requestAnimationFrame(updatePosition);
        }
    
        updatePosition();
    }
    
    lerp(start, end, progress) {
        return start + (end - start) * progress;
    }

    start(){
        game.started = true;
        //Something needs to happen here to assign the pawns properly.
        //We're going to check the pawns' position and assign them a value.
        container.classList.remove('container-waiting');
        if(document.querySelector('.loading')){
            document.querySelector('.loading').remove();
        }
        document.querySelector('.loading-bar').classList.add('loading-bar-hidden');
        document.querySelector('.game-button-start').classList.add('game-button-hidden');
        document.querySelector('.game-button-leave').classList.add('game-button-hidden');
        document.querySelector('.game-button-perspective').classList.remove('game-button-hidden');

        this.showCoordinates();
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

(async () => {
    game.spiritMoves = await fetchSpiritMoves();
})();

const loadingDivElement = document.createElement('div');
loadingDivElement.className = 'loading';
container.appendChild(loadingDivElement);

function animate() { //this function is called every frame by ThreeJS (by default)
    requestAnimationFrame(animate);
    if(game == undefined){
        return;
    }
    game.scene.traverse((child) => {
        if (child.isMesh && child.material.uniforms) {
            child.material.uniforms.time.value += 0.05;
        }
    });
    openCubes.forEach(cube => {
        if(cube.onitamaType == 'open'){
            cube.material.opacity = 0;
        }
    });
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

async function fetchTable(){
    if(currentApi == ""){ //Do nothing if the API has not been resolved yet
        return;
    }
    clearInterval(tableFetchInterval);
    if(game.started == true){ //Do nothing if the game has already started
        return;
    }
    if(gameId != undefined && gameId != "null"){ //"start" the game if the gameID exists and is set properly
        clearInterval(tableFetchInterval);
        game.id = gameId;
        console.log(game.id);
        await getGame();
        game.start();
        return;
    }
    tableFetchInterval = setInterval(async () => {
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
        let usersThatLeft;

        if(table.ownerPlayerId != user.id){ //Only show the "start game" button to the owner
            document.querySelector('.game-button-start').classList.add('game-button-hidden');
        } else {
            document.querySelector('.game-button-start').classList.remove('game-button-hidden');
        }

        let totalPlayers = data.seatedPlayers;
        if(allUsers != undefined){
            if(allUsers.length != totalPlayers.length){
                //The list of players has changed. Let's check who
                if(allUsers.length > totalPlayers.length){
                    //A user has left
                    for (let i = 0; i < allUsers.length; i++) {
                        const name = allUsers[i].name;
                        if(totalPlayers.some(obj => obj.name != name)){
                            console.log(name + " HAS LEFT");
                            throw_floating_message(name + ' has left the table.', 'Notice!');
                            setTimeout(() => {
                                floatingError.classList.add('floating-error-hidden');
                            }, 1250);
                        }
                    }
                } else {
                    //A user has joined
                    for (let i = 0; i < totalPlayers.length; i++) {
                        const name = totalPlayers[i].name;
                        if(allUsers.some(obj => obj.name != name)){
                            console.log(name + " HAS JOINED");
                            throw_floating_message(name + ' has joined the table!', 'Notice!');
                            setTimeout(() => {
                                floatingError.classList.add('floating-error-hidden');
                            }, 1250);
                        }
                    }
                }
            }
        }
        allUsers = totalPlayers;
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
                            game.currentPlayerObject = player;
                        }
                    } else if (player.direction == "South"){
                        player.facing = 0;
                        player.number = 2;
                        game.team2 = player;
                        if(user.id == player.id){
                            game.currentPlayer = 2;
                            game.currentPlayerObject = player;
                            game.setCamera();

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
                if(data.preferences.tableType == "wotw"){
                    game.loadSpirit();
                }
                //Make sure the current player can only control his own pawns, and only when it is his turn.
                //Move this to its own function
            } else {
                container.classList.remove('container-waiting');
                container.classList.add('container-loading');
            }

        setTimeout(fetchTable, 500);
        } else if(game.id == undefined || game.id == "00000000-0000-0000-0000-000000000000") {
            game.id = table.gameId;
            console.log(game.id);
            //setTimeout(fetchTable, 500);
        } else {
            clearInterval(tableFetchInterval);
            console.log(game.id);
            getGame();
            game.start();
        }
        //setTimeout(fetchTable, 2500);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });
}, 1500);
}
fetchTable();
async function getGame(){
    clearInterval(getGameInterval);
    getGameInterval = setInterval(async () => {
    const response = await fetch(currentApi + "/api/Games/" + game.id, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    }).then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw_floating_error(errorData.message, '400', "#c60025");
            });
        } else {
        }
        return response.json();
    }).then(data => {
        game.gameType = data.gametype;
        if(data.gametype != "blitz"){
            document.querySelector('.player-time').classList.add('game-cards-time-hidden');
            document.querySelector('.enemy-time').classList.add('game-cards-time-hidden');
        } else {
            document.querySelector('.player-time').classList.remove('game-cards-time-hidden');
            document.querySelector('.enemy-time').classList.remove('game-cards-time-hidden');
        }
        if(data.winnerPlayerId != "00000000-0000-0000-0000-000000000000" && data.winnerPlayerId != undefined){
            console.log("The game has been won!");
            console.log(data.winnerPlayerId);
            console.log(data.winnerMethod);
            let winnerName;
            data.players.forEach(player => {
                if(player.id == data.winnerPlayerId){
                    winnerName = player.name;
                }
                if(player.id == user.id){
                    //console.log("id matches");
                    if(game.gameType == "competitive"){
                        getUser();
                    }
                }
            });
            document.querySelector('.game-over-subtitle').textContent = "The game has been won by " + winnerName + " by " + data.winnerMethod + ".";
            document.querySelector('.game-over').classList.remove('game-over-hidden');
        }
        if(game.started == false){
            localStorage.setItem("gameId", data.id);
            //Set game board with actual data
            const grid = data.playMat.grid;
            for (let i = 0; i < grid.length; i++) {
                for (let j = 0; j < grid[i].length; j++) {
                    let item = grid[i][j];
                    game.board.currentBoard[i][j][2] = item;
                }
            }
        } else {
            
            //Set gameCards with actual data
            let enemyName;
            let enemyCards;
            let enemyColor;
            let enemyTime;
            let playerCards;
            let playerColor;
            let playerTime;
            let extraCard = data.extraMoveCard;
            const enemyCardElements = document.querySelectorAll('.enemy-card-blocks .block');
            const enemyAltCardElements = document.querySelectorAll('.enemy-card-blocks-alt .block');
            const playerCardElements = document.querySelectorAll('.player-card-blocks .block');
            const playerAltCardElements = document.querySelectorAll('.player-card-blocks-alt .block');
            const extraCardElements = document.querySelectorAll('.extra-card-blocks .block');
            const extraAltCardElements = document.querySelectorAll('.extra-card-blocks-alt .block');
            //console.log(data);
            data.players.forEach(player => {
                if(player.id == data.playerToPlayId && user.id != data.playerToPlayId){
                    //Set enemy name
                    enemyName = player.name;
                    enemyCards = player.moveCards;
                    enemyColor = player.color;
                    enemyTime = player.time;
                } else {
                    //Enemy is the one after us.
                    for (let i = 0; i < data.players.length; i++) {
                        const player = data.players[i];
                        if(player.id == user.id){
                            let num = i+1;
                            if(i+1 >= data.players.length){
                                num = 0;
                            }
                            enemyName = data.players[num].name;
                            enemyTime = data.players[num].time;
                            enemyCards = data.players[num].moveCards;
                            enemyColor = data.players[num].color;
                        }
                    }
                }
                //onsole.log("ENEMY CARDS");
                //console.log(enemyCards);
                //console.log("Enemy time: " + enemyTime);
                document.querySelector('.enemy-time').textContent = "(" + enemyTime +  "s)"
                if(player.id == user.id){
                    playerCards = player.moveCards;
                    playerColor = player.color;
                    game.playerCards = playerCards;
                    playerTime = player.time;
                    //console.log("Your time: " + playerTime);
                    document.querySelector('.player-time').textContent = "(" + playerTime +  "s)"
                }
            });
            document.querySelector('.enemy-name').textContent = enemyName;
            document.querySelector('.enemy-name').style.background = enemyColor;
            document.querySelector('.player-name').style.background = playerColor;
            if(enemyCards != undefined){
                let count = 0;
                let count2 = 0;
                for (let i = 0; i < enemyCards.length; i++) {
                    const card = enemyCards[i];
                    const grid = enemyCards[i].grid.reverse();
                    const altGrid = enemyCards[i].altGrid.reverse(); //For normal cards, this is an empty array
                    document.querySelectorAll('.enemy-card-name')[i].textContent = card.name;
                    for (let j = 0; j < grid.length; j++) {
                        for (let k = 0; k < grid[j].length; k++) {
                            let block = enemyCardElements[count];
                            if(block == undefined){
                                break;
                            }
                            count++;
                            if(grid[j][k] == "0"){
                                block.style.background = "#37383c";
                            } else if(grid[j][k] == "1"){
                                block.style.background = enemyCards[i].stampColor;
                            } else {
                                block.style.background = "black";
                            }
                        }
                    }
                    if(altGrid.length > 0){
                        //console.log(altGrid);
                        document.querySelectorAll('.enemy-card-blocks-alt')[i].classList.remove('player-card-blocks-hidden');
                        for (let j = 0; j < altGrid.length; j++) {
                            for (let k = 0; k < altGrid[j].length; k++) {
                                let block = enemyAltCardElements[i * 25 + count2];
                                if(block == undefined){
                                    break;
                                }
                                count2++;
                                if(altGrid[j][k] == "0"){
                                    block.style.background = "#363a3e";
                                } else if(altGrid[j][k] == "1"){
                                    block.style.background = enemyCards[i].stampColor;
                                } else {
                                    block.style.background = "lightblue";
                                }
                            }
                        }
                    } else {
                        document.querySelectorAll('.enemy-card-blocks-alt')[i].classList.add('player-card-blocks-hidden');
                    }
                }
            }
            if(playerCards != undefined){
                let count = 0;
                let count2 = 0;
                for (let i = 0; i < playerCards.length; i++) {
                    const card = playerCards[i];
                    const grid = playerCards[i].grid.reverse();
                    const altGrid = playerCards[i].altGrid.reverse(); //For normal cards, this is an empty array
                    if(document.querySelectorAll('.player-card')[i].classList.contains('player-card-selected')){
                        document.querySelectorAll('.player-card-name')[i].textContent = card.name + " (Active)";
                    } else {
                        document.querySelectorAll('.player-card-name')[i].textContent = card.name;
                    }
                    for (let j = 0; j < grid.length; j++) {
                        for (let k = 0; k < grid[j].length; k++) {
                            let block = playerCardElements[count];
                            if(block == undefined){
                                break;
                            }
                            count++;
                            if(grid[j][k] == "0"){
                                block.style.background = "#37383c";
                            } else if(grid[j][k] == "1"){
                                block.style.background = playerCards[i].stampColor;
                            } else {
                                block.style.background = "black";
                            }
                        }
                    }

                    if(altGrid.length > 0){
                        document.querySelectorAll('.player-card-blocks-alt')[i].classList.remove('player-card-blocks-hidden');
                        //console.log(altGrid);
                        for (let j = 0; j < altGrid.length; j++) {
                            for (let k = 0; k < altGrid[j].length; k++) {
                                let block = playerAltCardElements[i * 25 + count2];
                                if(block == undefined){
                                    break;
                                }
                                count2++;
                                if(altGrid[j][k] == "0"){
                                    block.style.background = "#363a3e";
                                } else if(altGrid[j][k] == "1"){
                                    block.style.background = playerCards[i].stampColor;
                                } else {
                                    block.style.background = "lightblue";
                                }
                            }
                        }
                    } else {
                        document.querySelectorAll('.player-card-blocks-alt')[i].classList.add('player-card-blocks-hidden');
                    }
                }
            }
            if(extraCard != undefined){
                //return;
                const card = extraCard;
                const grid = extraCard.grid.reverse();
                const altGrid = extraCard.altGrid.reverse();
                document.querySelector('.extra-card-name').textContent = card.name;
                let count = 0;
                for (let j = 0; j < grid.length; j++) {
                    for (let k = 0; k < grid[j].length; k++) {
                        let block = extraCardElements[count];
                        if(block == undefined){
                            break;
                        }
                        count++;
                        if(grid[j][k] == "0"){
                            block.style.background = "#37383c";
                        } else if(grid[j][k] == "1"){
                            block.style.background = extraCard.stampColor;
                        } else {
                            block.style.background = "black";
                        }
                    }
                }
                if(altGrid.length > 0){
                    document.querySelector('.extra-card-blocks-alt').classList.remove('player-card-blocks-hidden');
                    //console.log(altGrid);
                    let count = 0;
                    for (let j = 0; j < altGrid.length; j++) {
                        for (let k = 0; k < altGrid[j].length; k++) {
                            let block = extraAltCardElements[count];
                            if(block == undefined){
                                break;
                            }
                            count++;
                            if(altGrid[j][k] == "0"){
                                block.style.background = "#363a3e";
                            } else if(altGrid[j][k] == "1"){
                                block.style.background = extraCard.stampColor;
                            } else {
                                block.style.background = "lightblue";
                            }
                        }
                    }
                } else {
                    document.querySelector('.extra-card-blocks-alt').classList.add('player-card-blocks-hidden');
                }
            }
            //The game has started. Check playerToPlay
            if(user.id == data.playerToPlayId){
                //We can play
                //console.log("Your turn!");
                document.querySelectorAll('.player-card').forEach(element => {
                    element.classList.add('player-card-pointer');
                });
                game.playerToPlay = user.warriorName;
                document.querySelector('.toast').textContent = "Your turn";
                data.players.forEach(player => {
                    if(player.id == user.id){
                        //This is us
                        if(player.hasValidMoves == false){
                            //Add message telling us to skip move
                            document.querySelector('.skip-move').classList.remove('skip-move-hidden');
                        } else {
                            //Remove message telling us to skip move
                            document.querySelector('.skip-move').classList.add('skip-move-hidden');
                        }
                    }
                });
            } else {
                //Someone else's turn
                //Remove message telling us to skip move
                document.querySelector('.skip-move').classList.add('skip-move-hidden');
                for (let i = 0; i < data.players.length; i++) {
                    const player = data.players[i];
                    if(player.id == data.playerToPlayId){
                        //console.log(player.name+"'s turn!");
                        game.playerToPlay = player.name;
                    }
                }
                document.querySelectorAll('.player-card').forEach(element => {
                    element.classList.remove('player-card-pointer');
                    element.classList.remove('player-card-selected');
                    element.querySelector('.card-name').textContent = element.querySelector('.card-name').textContent.replace(" (Active)", "");
                });
                document.querySelector('.toast').textContent = game.playerToPlay + "'s turn";
            }
        }
        if(game.loaded == false){


            if(localStorage.getItem("previousBoard"+data.id) == null){
                localStorage.setItem("previousBoard"+data.id, JSON.stringify(data.playMat.grid));
            } else {
                previousBoard = JSON.parse(localStorage.getItem("previousBoard"+data.id));
            }

            let playerNorth;
            let playerSouth;
            let playerWest;
            let playerEast;
            for (let f = 0; f < data.players.length; f++) {
                let player = data.players[f];
                switch (player.direction) {
                    case 'North':
                        playerNorth = player;
                        break;
                    case 'South':
                        playerSouth = player;
                        break;
                    case 'West':
                        playerWest = player;
                        break;
                    case 'East':
                        playerEast = player;
                        break;
                
                    default:
                        break;
                }
            }

            for (let i = 0; i < game.board.currentBoard.length; i++) {
                for (let k = 0; k < game.board.currentBoard[i].length; k++) {
                    const coord = [i, k];
                    let index = ((coord[0] * 5) + coord[1]) % 5;
                    let count = game.board.currentBoard[i][k][0];
                    let pawn;
                    if(count == 0){
                        break;
                    }
                    if(count < 6){
                        pawn = playerNorth.school.allPawns[index];
                    } else if(count < 11){
                        pawn = playerSouth.school.allPawns[index];
                    } else if(count < 16){
                        pawn = playerWest.school.allPawns[index];
                    } else {
                        pawn = playerEast.school.allPawns[index];
                    }
                    //console.log(coord[0] + ", " + coord[1]);
                    //console.log(pawn.ownerId);
                    game.board.currentBoard[coord[0]][4 - coord[1]][6] = pawn;
                }
            }
            game.board.currentBoard[2][2][6] = data.playMat.grid[2][2];
            game.loaded = true;
        } else {
            // The game has loaded. Let's compare boards
            let grid = data.playMat.grid;
            if(previousBoard != undefined){
                const changes = detectChanges(previousBoard, grid);
                if(changes.length > 0){
                    console.log(changes);
                    //Now do changes
                    for (let i = 0; i < changes.length; i++) {
                        const change = changes[i];
                        if(change.type == "moved"){
                            //Move the pawn
                            game.movePawn3D(change);
                        } else if (change.type == "removed"){
                            console.log("Pawn Removed!");
                            game.removePawn3D(change);
                        } else if (change.type == "added"){
                            console.log(added);
                        }
                    }
                }
            }
            previousBoard = grid;
        }
        //Get game again
        //setTimeout(getGame, 500);
    }).catch(error => {
        console.log(error);
        throw_floating_error(error, '500', "#c60025");
    });

}, 1000);
}
window.fetchTable = fetchTable;

function detectChanges(oldArray, newArray) {
    const changes = [];

    // Iterate through each element in the arrays
    for (let i = 0; i < oldArray.length; i++) {
        for (let j = 0; j < oldArray[i].length; j++) {
            // If there's no object in the old array but there is one in the new array
            if (!oldArray[i][j] && newArray[i][j]) {
                changes.push({
                    type: 'added',
                    object: newArray[i][j],
                    to: { row: i, column: j }
                });
            }
            // If there's an object in the old array but not in the new array
            else if (oldArray[i][j] && newArray[i][j] == null) {
                changes.push({
                    type: 'removed',
                    object: oldArray[i][j],
                    from: { row: i, column: j }
                });
            }
            // If there's an object in both arrays but with different properties
            else if (oldArray[i][j] && newArray[i][j] && JSON.stringify(oldArray[i][j]) !== JSON.stringify(newArray[i][j])) {
                changes.push({
                    type: 'moved',
                    object: newArray[i][j],
                    from: { row: i, column: j },
                    to: { row: newArray[i][j].Position.Row, column: newArray[i][j].Position.Column }
                });
            }
        }
    }

    return changes;
}

function compareElo(newElo){
    if(game.oldElo == undefined || game.oldElo == null){
        game.oldElo = newElo;
    }
    let oldElo = game.oldElo;
    if(oldElo < newElo){
        let diff = newElo - oldElo;
        console.log("DIFFERENCE: " + diff);
        let string = "Your ELO has been increased by " + diff + " points, which sets it at " + newElo + " points total.";
        document.querySelector('.game-over-elo').textContent = string;
        document.querySelector('.game-over-elo').classList.remove("game-over-elo-hidden");
    } else if (oldElo > newElo){
        let diff = oldElo - newElo;
        console.log("DIFFERENCE: " + diff);
        let string = "Your ELO has been reduced by " + diff + " points, which sets it at " + newElo + " points total.";
        document.querySelector('.game-over-elo').textContent = string;
        document.querySelector('.game-over-elo').classList.remove("game-over-elo-hidden");
    } else {
    }
}

async function getUser(){
    try {
        const response = await fetch(currentApi + "/api/Authentication/refresh", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({})
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error("Your token has expired already!");
        }
        
        const data = await response.json();
        //console.log(data);
        compareElo(data.user.elo)
    } catch(error) {
        //console.log(error);
    }
}

document.querySelector('.skip-move-button').addEventListener('click', () => {
    let cardSelected = false;
        const playerCardElements = document.querySelectorAll('.player-card');
        for (let i = 0; i < playerCardElements.length; i++) {
            const element = playerCardElements[i];
            if(element.classList.contains('player-card-selected')){
                cardSelected = true;
            }
        }
    if(cardSelected == false){
        playerCardElements.forEach(card => {
            card.classList.remove('player-card-selected');
            card.querySelector('.card-name').textContent = card.querySelector('.card-name').textContent.replace(" (Active)", "");
        });
        playerCardElements[0].classList.add('player-card-selected');
        game.selectedCard = playerCardElements[0].querySelector('.card-name').textContent;
        playerCardElements[0].querySelector('.card-name').textContent += " (Active)";
        cardSelected = true;
    }
    console.log(game.selectedCard);
    skipMove();
});

async function skipMove(){
    try {
        const response = await fetch(currentApi + "/api/Games/" + game.id + "/skip-movement", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({"moveCardName": game.selectedCard})
        });
        
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error("Your token has expired already!");
        }
        
        const data = await response.json();
        console.log(data);
    } catch(error) {
        //console.log(error);
    }
}


class ClickHandler {
    constructor() {
        this.clickStart = 0;
        this.clickPosition = { x: 0, y: 0 };
    }

    startClick(event) {
        if(game === undefined){
            return;
        }
        // Record the start time
        this.clickStart = performance.now();
        this.hover(event);
    }

    hover(event){
        this.clickPosition.x = event.clientX || event.touches[0].clientX;
        this.clickPosition.y = event.clientY || event.touches[0].clientY;
        lastPointerCoords = { x: this.clickPosition.x, y: this.clickPosition.y };
        // Set the mouse coordinates in the game object
        game.mouseX = this.clickPosition.x;
        game.mouseY = this.clickPosition.y;
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
            let complete = false;
            intersects.forEach(element => {
                let object = element.object;
                if(game.lastClicked == undefined){
                    //console.log("Undefined");
                    game.lastClicked = "pawn";
                }
                //console.log(game.lastClicked);
                if(object.onitamaType == "selectable" && object.name.includes("selectable")){
                    //console.log("We're returning: ")
                    //console.log(object);
                    document.body.style.cursor = 'pointer';
                    if(game.playerToPlay != user.warriorName){
                        return;
                    }
                    object.material.opacity = 0.8;
                    cubes++;
                    hovering = true;
                    //console.log(object);
                    hoveredCube = object;
                    complete = true;
                }
            });
            if(complete == true){
                return;
            }
            intersects.forEach(element => {
                let object = element.object;
                if(object.name.includes(game.currentPlayer+"hover")){
                    document.body.style.cursor = 'pointer';
                    if(game.playerToPlay != user.warriorName){
                        return;
                    }
                    object.material.opacity = 0.8;
                    cubes++;
                    hovering = true;
                    //console.log(object);
                    hoveredCube = object;
                }
            });
        } else {
        }
    }

    endClick() {
        // Calculate the click duration
        const clickEnd = performance.now();
        const clickDuration = clickEnd - this.clickStart;

        // Log the duration to the console
        console.log(`Click duration: ${clickDuration} ms`);
        if(clickDuration < 200){
            //Click detected
            console.log(`Click position: (${this.clickPosition.x}, ${this.clickPosition.y})`);
            onClick();
        }
        // Reset the start time
        this.clickStart = 0;
    }
}

var clickHandler = new ClickHandler();

function resetHighlightedObjects() {
    // Reset opacity for all objects with the name "hover"
    game.scene.traverse(function(child) {
        if (child instanceof THREE.Mesh && child.name.includes("hover")) {
            child.material.opacity = 0.3; // Reset opacity to default
            child.material.needsUpdate = true; // Update material
            document.body.style.cursor = 'default';
            hoveredCube = undefined;
        }
    });
    if(clickedCube != undefined){
        if(game.playerToPlay != user.warriorName){
            return;
        }
        clickedCube.material.opacity = 0.8;
    }
    hovering = false;
}

function simulatePointerMove() {
    // Create a new MouseEvent object
    let event = new MouseEvent('pointermove', {
        bubbles: true,
        cancelable: true,
        clientX: lastPointerCoords.x,
        clientY: lastPointerCoords.y
    });

    // Dispatch the event on the window object
    window.dispatchEvent(event);
}

async function onClick(){
    if(game.playerToPlay != user.warriorName){
        return;
    }
    if(hovering == true){
        console.log(hoveredCube.name); //Click detected!
        clickedCube = hoveredCube;
        if(clickedCube.onitamaType == "selectable"){
            allCubes.forEach(cube => {
                game.scene.remove(cube);
            });
            simulatePointerMove();

            //Now we need to actually do the move
            let pawnCube = selectedPawn;
            let selectCube;
            let selectedPosition;
            allCubes.forEach(cube => {
                if(cube == clickedCube){
                    selectCube = cube;
                    const column = 4 - ((4 - cube.position.x) / 2);
                    const row = ((cube.position.z + 4) / 2);
                    selectedPosition = [row, column];
                }
            });
            selectionCoords = [selectedPosition[0], 4 - selectedPosition[1]];
            console.log(pawnCoords);
            console.log(selectionCoords);
            let pawn = game.board.currentBoard[pawnCoords[0]][pawnCoords[1]][6];
            console.log(pawn.id + " : " + pawn.ownerId);
            let cardName = game.selectedCard;

            console.log(selectionCoords);
            let pawnId = pawn.id;
            if(pawn.id == undefined){
                pawnId = pawn.Id;
            }
            //First check if the card is a Spririt Card
            //If it is, check if the first move has been selected. If not, check if pawn is spirit. If so, let the user know to pick a normal pawn
            const isCardSpirit = game.spiritMoves.some(obj => {
                    return obj.Name.includes(cardName);
            });
            if(isCardSpirit){
                //The card is a spirit card. Check if first move has been selected
                if(game.selectedMove == undefined){
                    //The first move has not been selected. That means the pawn should be a regular pawn
                    if(pawn.Type == 2){
                        //The pawn is a Spirit. Do nothing (Let the  user know to pick a normal pawn)
                        console.log("You must select a regular pawn first");
                        throw_floating_error("Your must select a regular pawn/move first", "", "");
                        return;
                    }
                    //The pawn is not a spirit. Continue
                    game.selectedMove = [pawnId, cardName, selectionCoords];
                    console.log("Selecting move: ", game.selectedMove);
                    console.log(pawn);
                    return;
                } else {
                    //The first move has already been selected. Now time for the spirit move
                    if(pawn.Type != 2){
                        //The pawn is a Spirit. Do nothing (Let the  user know to pick a normal pawn)
                        console.log("You must select a Spirit pawn now");
                        throw_floating_error("You must select a Spirit pawn/move now", "", "");
                        return;
                    }
                    game.selectedMoveSpirit = [pawnId, cardName, selectionCoords];
                    //Do a special movePawn method here that does the 2 moves
                    console.log("Doing move: ", game.selectedMoveSpirit);
                    game.movePawnSpirit();
                    return;
                }
            }
            //A normal move was selected. Void
            game.selectedMove = undefined;
            game.selectedMoveSpirit = undefined;
            game.movePawn(pawnId, cardName, selectionCoords);
            return;
        }
        //Check if a card has been selected. If not, pick the first card.
        let cardSelected = false;
        const playerCardElements = document.querySelectorAll('.player-card');
        for (let i = 0; i < playerCardElements.length; i++) {
            const element = playerCardElements[i];
            if(element.classList.contains('player-card-selected')){
                cardSelected = true;
            }
        }
        if(cardSelected == false){
            playerCardElements.forEach(card => {
                card.classList.remove('player-card-selected');
                card.querySelector('.card-name').textContent = card.querySelector('.card-name').textContent.replace(" (Active)", "");
            });
            playerCardElements[0].classList.add('player-card-selected');
            game.selectedCard = playerCardElements[0].querySelector('.card-name').textContent;
            playerCardElements[0].querySelector('.card-name').textContent += " (Active)";
            cardSelected = true;
        }
        resetHighlightedObjects();
        simulatePointerMove();
        //We need to know possible moves. Luckily for us, we don't need to rotate anything here.
        //When we send the move to the API though, we probably will have to rotate the data.
        let startCoords;
        let board = game.board.currentBoard;
        for (let i = 0; i < board.length; i++) {
            for (let j = 0; j < board[i].length; j++) {
                const coord = [i, j];
                let cube;
                if(board[i][j] != undefined){
                    cube = board[i][j][5];
                    if(cube != undefined){
                        if(clickedCube.name == cube.name){
                            startCoords = [i, j];
                        }
                    }
                }
                
            }
        }
        let selectedCard;
        for (let i = 0; i < game.playerCards.length; i++) {
            const card = game.playerCards[i];
            if(card.name == game.selectedCard){
                selectedCard = card;
            }
        }

        if(hoveredCube.onitamaType == "pawn"){
            console.log(startCoords[0]);
            console.log(4 - startCoords[1]);
            selectedPawn = hoveredCube;
            pawnCoords = startCoords;
        }

        openCubes.forEach(cube => {
            cube.onitamaType = "open";
        });
        if(startCoords != undefined && selectedCard != undefined && board[startCoords[0]][startCoords[1]][6] != undefined){
            let pawnId;
            let pawnJson;
            if(board[startCoords[0]][startCoords[1]][0] == 200){
                pawnId = board[startCoords[0]][startCoords[1]][6].Id;
                pawnJson =board[startCoords[0]][startCoords[1]][6];
            } else {
                pawnId = board[startCoords[0]][startCoords[1]][6].id;
                pawnJson = board[startCoords[0]][startCoords[1]][6];
            }
            if(pawnJson.Type === 2){
                game.lastClicked = "spirit";
                console.log("Spirit");
            } else {
                console.log(pawnJson);
                game.lastClicked = "pawn";
                console.log("Pawn");
            }
            console.log(pawnId);
            const coordinates = await game.getPossibleCoordinate(pawnId);
            allCubes.forEach(cube => {
                    game.scene.remove(cube);
            });
            if(coordinates != undefined){
                coordinates.forEach(el => {
                    let row = el.to.row;
                    let column = el.to.column;
                    console.log(row + " : " + column);
                    console.log("Identity: 0")
                    const material = new THREE.MeshBasicMaterial({
                        color: selectedPawn.material.color, //White
                        transparent: true, // Transparent
                        opacity: 0.8, // Opacity to 0.1
                    });
    
                    const cubeGeometry = new THREE.BoxGeometry(1.25, 0.05, 1.25);
    
                    // Create a mesh using the geometry and material
                    const cube = new THREE.Mesh(cubeGeometry, material);
                    cube.name = "selectable";
                    cube.position.x = 4 - (column * 2);
                    cube.position.z = row * 2 - 4;
                    cube.scale.x = 1.05;
                    cube.scale.z = 1.05;
                    cube.scale.y = 1.05;
                    game.scene.add(cube);
                    cube.onitamaType = "selectable";
                    allCubes.push(cube);
                });
            }
        }
    }
}

window.addEventListener('touchstart', (event) => clickHandler.startClick(event), false);
window.addEventListener('touchend', () => clickHandler.endClick(), false);
window.addEventListener('mousedown', (event) => clickHandler.startClick(event), false);
window.addEventListener('mouseup', () => clickHandler.endClick(), false);

window.addEventListener('pointermove', (event) => clickHandler.hover(event), false);
window.addEventListener('touchmove', (event) => clickHandler.hover(event), false);

document.querySelector('.game-button-perspective').addEventListener('click', () => {
    if(game == null){
        return;
    }
    if(game.currentPlayer == null){
        return;
    }
    game.setCamera();
});