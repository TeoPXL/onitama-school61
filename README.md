# Onitama61

Onitama61 is a web application that allows players to enjoy the Onitama board game in 3D with online and local multiplayer options, as well as the ability to play against an AI opponent.

## Table of Contents

- [Installation](#installation)
  - [Run Locally](#run-locally)
  - [Run Online](#run-online)
- [Usage](#usage)
- [Credits](#credits)
- [License](#license)

## Installation

### Run Locally

To run the project locally, follow these steps:

1. Clone the repository.
2. Navigate to the `/frontend` directory and run a web server.
3. Open the backend solution in Visual Studio and run it. Note that the backend uses LocalDB, which requires MSSQL to be installed.

### Run Online

To run the project online, follow these steps:

1. Fork this repository.
2. Edit the domain name in `/.github/workflows/web.yml` to match your desired domain name.
3. Deploy the frontend using the GitHub Pages workflow.
4. Host the backend on any Windows server. The included workflow is designed to work with Microsoft Azure App Service running ASP.NET Core (.NET 8.0 LTS) with CI/CD.
5. Ensure you update the remote API URL in `/frontend/js/shared.js` to point to your remote API server.
6. On your remote server, add the following environment variable / connection string: "OnitamaDbConnection"="server=YOUR_DATABASE_URL;database=YOUR_DATABASE_NAME>;uid=YOUR_USERNAME;password=YOUR_PASSWORD;"

Note that the backend currently supports both MS SQL Server and PostgreSQL databases.

## Usage

Once the project is set up, users can:

- Create an account
- Log in
- Create tables for various game types
- Join available tables
- Start games once the table is full

For more information on how to play Onitama, refer to the [official rules](https://www.arcanewonders.com/wp-content/uploads/2021/05/Onitama-Rulebook.pdf).

## Credits

Much of the backend code for this project was provided by PXL University of Applied Sciences and Arts.

## License

This project is licensed under the MIT License.
