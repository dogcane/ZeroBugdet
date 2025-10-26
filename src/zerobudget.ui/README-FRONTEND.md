# ZeroBudget Frontend

Angular frontend application for ZeroBudget API with Material Design components.

## Features

- **Authentication Flow**
  - Automatic check for main user existence
  - Main user registration
  - User login with JWT authentication
  - Protected routes with auth guards

- **User Management** (Main User Only)
  - View all registered users
  - Invite new users via email
  - View and manage pending invitations
  - Delete users (except main user)

- **Bucket Management**
  - List all budget buckets
  - Create new buckets with name, description, and default limit
  - Edit existing buckets
  - Enable/Disable buckets
  - Delete buckets

- **Spending Management**
  - View all spendings
  - Create new spending entries with:
    - Bucket selection
    - Date picker
    - Description
    - Amount
    - Owner
    - Tags (multiple)
  - Delete spendings

- **Monthly View**
  - Select year and month
  - View monthly bucket allocations
  - See monthly spending totals
  - Track remaining budget per bucket
  - Visual indicators for over/under budget
  - Generate monthly data from templates

## Technology Stack

- **Angular 18** - Frontend framework
- **Angular Material** - UI component library with Purple/Green theme
- **RxJS** - Reactive programming
- **TypeScript** - Type-safe development
- **SCSS** - Styling

## Prerequisites

- Node.js 20.x or higher
- npm 10.x or higher
- ZeroBudget API running on `https://localhost:7000`

## Installation

```bash
cd src/zerobudget.ui
npm install
```

## Configuration

The API URL is currently hardcoded in the service files. Update the following files to match your API endpoint:

- `src/app/services/auth.service.ts`
- `src/app/services/bucket.service.ts`
- `src/app/services/spending.service.ts`
- `src/app/services/monthly.service.ts`

Replace `https://localhost:7000` with your API base URL.

## Development Server

```bash
npm start
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Build

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

## Application Structure

```
src/app/
├── components/
│   ├── auth/              # Authentication components
│   │   ├── login/
│   │   ├── register-main-user/
│   │   └── initial-check.component.ts
│   ├── buckets/           # Bucket management
│   │   ├── bucket-list/
│   │   └── bucket-form/
│   ├── spendings/         # Spending management
│   │   ├── spending-list/
│   │   └── spending-form/
│   ├── monthly/           # Monthly view
│   │   └── monthly-view/
│   ├── users/             # User management
│   │   └── user-list/
│   └── layout/            # Layout components
│       └── main-layout/
├── services/              # API services
│   ├── auth.service.ts
│   ├── bucket.service.ts
│   ├── spending.service.ts
│   └── monthly.service.ts
├── models/                # TypeScript interfaces
│   ├── user.model.ts
│   ├── bucket.model.ts
│   ├── spending.model.ts
│   └── monthly.model.ts
├── guards/                # Route guards and interceptors
│   ├── auth.guard.ts
│   └── auth.interceptor.ts
└── app.routes.ts          # Application routes
```

## Usage

### First Time Setup

1. Start the application
2. You will be automatically redirected to register the main user if none exists
3. Create the main user account
4. Login with your credentials

### Main User Capabilities

As the main user, you can:
- Access all features
- Manage other users
- Invite new users
- View all data

### Regular User Capabilities

Regular users can:
- View and manage buckets
- Create and view spendings
- View monthly summaries

## Routes

- `/` - Initial check (redirects to login or register)
- `/login` - User login
- `/register-main-user` - Main user registration
- `/monthly` - Monthly view (authenticated)
- `/buckets` - Bucket list (authenticated)
- `/spendings` - Spending list (authenticated)
- `/spendings/new` - Create new spending (authenticated)
- `/users` - User management (main user only)

## Authentication

The application uses JWT token-based authentication:
- Tokens are stored in localStorage
- HTTP interceptor automatically adds the token to all API requests
- Auth guard protects routes requiring authentication
- Main user guard restricts access to user management

## Material Design Theme

The application uses Angular Material's purple-green theme with:
- Primary color: Purple
- Accent color: Green
- Dark mode support enabled

## Notes

- API URLs are currently hardcoded - consider moving to environment files
- CORS must be enabled on the API server
- The API must be running and accessible before starting the frontend
- Font inlining is disabled to avoid internet dependency during build

## Future Enhancements

- Environment-based configuration
- Improved error handling
- Loading states and spinners
- Form validation improvements
- Responsive design optimizations
- Pagination for large lists
- Search and filter capabilities
- Export/Import functionality
- Charts and data visualization
