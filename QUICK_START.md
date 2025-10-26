# ZeroBudget - Quick Start Guide

This guide helps you get the ZeroBudget application (API + Frontend) running quickly.

## Prerequisites

- .NET 8 SDK
- Node.js 20.x or higher
- PostgreSQL database
- Git

## Step 1: Clone the Repository

```bash
git clone https://github.com/dogcane/ZeroBugdet.git
cd ZeroBugdet
```

## Step 2: Set Up the Database

1. Install PostgreSQL if not already installed
2. Create a database for ZeroBudget
3. Update connection string in API configuration

## Step 3: Start the API

```bash
# Navigate to the API project
cd src/zerobudget.core/src/zerobudget.core/zerobudget.core.webapi

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project ../zerobudget.core.infrastructure.data

# Run the API
dotnet run
```

The API should now be running on `https://localhost:7000`

## Step 4: Start the Frontend

Open a new terminal window:

```bash
# Navigate to the frontend project
cd src/zerobudget.ui

# Install dependencies
npm install

# Start the development server
npm start
```

The frontend should now be running on `http://localhost:4200`

## Step 5: Access the Application

1. Open your browser and navigate to `http://localhost:4200`
2. You'll be prompted to register the main user (first time only)
3. Create your main user account
4. Login with your credentials
5. Start using the application!

## Default Configuration

- **API URL**: https://localhost:7000
- **Frontend URL**: http://localhost:4200
- **Database**: PostgreSQL (configure connection string)

## Troubleshooting

### API Issues

- **Database connection error**: Check your connection string in `appsettings.json`
- **Port already in use**: Change the port in `launchSettings.json`
- **CORS errors**: CORS should be configured in the API

### Frontend Issues

- **Can't connect to API**: Ensure API is running on https://localhost:7000
- **Build errors**: Run `npm install` again
- **Port 4200 in use**: Use `npm start -- --port 4201`

## Key Features

1. **Authentication**: JWT-based authentication
2. **Bucket Management**: Create and manage budget categories
3. **Spending Tracking**: Record and categorize expenses
4. **Monthly View**: See your budget status by month
5. **User Management**: Invite and manage users (main user only)

## Next Steps

- Read the [Frontend README](src/zerobudget.ui/README-FRONTEND.md) for detailed frontend documentation
- Check API documentation at `https://localhost:7000/scalar/v1` (when API is running)
- Review the [Implementation Summary](FRONTEND_IMPLEMENTATION_SUMMARY.md)

## Support

For issues or questions, please create an issue on the GitHub repository.
