# SaveLog Viewer

A modern web application for viewing and analyzing savelog files. This application provides a user-friendly interface for uploading, processing, and analyzing log files with features like timeline visualization, filtering, and searching.

## Features

- Upload and process savelog files
- View logs in a timeline with severity-based coloring
- Filter logs by various criteria (Context Folder ID, Study UID, etc.)
- Search through log contents
- Merge client and server logs based on Process ID
- Visualize workflow using Taskflow ID and Context Folder ID
- Interactive timeline with zoom capabilities

## Technology Stack

### Backend
- .NET Core 7.0
- C#
- Entity Framework Core
- SignalR for real-time updates

### Frontend
- Angular 17
- Bootstrap 5
- Chart.js for timeline visualization
- ng-bootstrap for UI components

## Prerequisites

- .NET Core SDK 7.0 or later
- Node.js 18.x or later
- npm 9.x or later

## Setup Instructions

1. Clone the repository:
```bash
git clone <repository-url>
cd SaveLogViewer
```

2. Set up the backend:
```bash
cd SaveLogViewer.API
dotnet restore
dotnet build
```

3. Set up the frontend:
```bash
cd ../SaveLogViewer.UI
npm install
```

4. Start the backend server:
```bash
cd ../SaveLogViewer.API
dotnet run
```

5. Start the frontend development server:
```bash
cd ../SaveLogViewer.UI
npm start
```

The application will be available at:
- Frontend: http://localhost:4200
- Backend API: https://localhost:5001

## Usage

1. Upload a savelog file using the "Upload New" button
2. Wait for the processing to complete
3. Select the processed log from the list
4. Use the timeline at the bottom to navigate through log entries
5. Use filters and search to find specific log entries
6. Click on timeline points to view detailed log information

## Configuration

### Backend Configuration
- Edit `appsettings.json` to configure:
  - File storage location
  - Maximum file size
  - Processing options

### Frontend Configuration
- Edit `environment.ts` to configure:
  - API endpoint
  - Other environment-specific settings

## Development

### Adding New Features
1. Create feature branch
2. Implement changes
3. Add tests
4. Create pull request

### Building for Production
```bash
# Build backend
cd SaveLogViewer.API
dotnet publish -c Release

# Build frontend
cd ../SaveLogViewer.UI
npm run build --prod
```

## Deployment

### IIS Deployment
1. Publish the backend to IIS
2. Configure the application pool
3. Deploy the frontend build to a web server
4. Configure CORS settings if needed

## License

This project is licensed under the MIT License - see the LICENSE file for details. 