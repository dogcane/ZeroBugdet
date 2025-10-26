# ZeroBudget Frontend - Application Flow

## Application Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ZeroBudget Frontend                       │
│                     (Angular 18)                             │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTP/HTTPS
                            │ JWT Bearer Token
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    ZeroBudget API                            │
│                    (.NET 8 / ASP.NET Core)                   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    PostgreSQL Database                        │
└─────────────────────────────────────────────────────────────┘
```

## User Flow

### First Time User

```
Start Application
    │
    ▼
Initial Check (/)
    │
    ├─ Main User Exists? ──NO──▶ Register Main User (/register-main-user)
    │                                    │
    │                                    ▼
    └─ Main User Exists? ──YES──▶ Login (/login)
                                         │
                                         ▼
                                   Authenticated Home
                                         │
                                         ▼
                                   Main Layout (with navigation)
```

### Authenticated User Journey

```
Main Layout
    │
    ├─▶ Monthly View (/monthly)
    │     └─ Select Year/Month
    │     └─ View Budget Status
    │     └─ See Spendings
    │     └─ Generate Monthly Data
    │
    ├─▶ Buckets (/buckets)
    │     └─ List All Buckets
    │     └─ Create New Bucket
    │     └─ Edit Bucket
    │     └─ Delete Bucket
    │
    ├─▶ Spendings (/spendings)
    │     └─ List All Spendings
    │     └─ View by Bucket/Owner
    │     └─ Delete Spending
    │
    ├─▶ New Spending (/spendings/new)
    │     └─ Select Bucket
    │     └─ Enter Details
    │     └─ Add Tags
    │     └─ Submit
    │
    └─▶ Users (/users) [Main User Only]
          └─ View All Users
          └─ Invite New User
          └─ View Invitations
          └─ Delete User
```

## Component Structure

```
App Component (Root)
    │
    └─▶ Router Outlet
         │
         ├─▶ Initial Check Component
         │
         ├─▶ Login Component
         │
         ├─▶ Register Main User Component
         │
         └─▶ Main Layout Component
              ├─▶ Toolbar (Top)
              │    ├─ App Title
              │    ├─ User Email
              │    └─ Logout Button
              │
              ├─▶ Sidenav (Left)
              │    ├─ Monthly View Link
              │    ├─ Buckets Link
              │    ├─ Spendings Link
              │    ├─ New Spending Link
              │    └─ Users Link (Main User Only)
              │
              └─▶ Content Area (Right)
                   ├─▶ Monthly View Component
                   ├─▶ Bucket List Component
                   │    └─▶ Bucket Form Dialog
                   ├─▶ Spending List Component
                   ├─▶ Spending Form Component
                   └─▶ User List Component
```

## Service Layer

```
Components
    │
    ├─▶ Auth Service
    │    ├─ Login/Logout
    │    ├─ Register Main User
    │    ├─ Token Management
    │    ├─ User Management
    │    └─ Invite Users
    │
    ├─▶ Bucket Service
    │    ├─ Get Buckets
    │    ├─ Create Bucket
    │    ├─ Update Bucket
    │    ├─ Delete Bucket
    │    └─ Enable Bucket
    │
    ├─▶ Spending Service
    │    ├─ Get Spendings
    │    ├─ Create Spending
    │    ├─ Update Spending
    │    ├─ Delete Spending
    │    └─ Enable Spending
    │
    └─▶ Monthly Service
         ├─ Get Monthly Buckets
         ├─ Get Monthly Spendings
         └─ Generate Monthly Data
```

## Data Flow

```
User Action (Component)
    │
    ▼
Service Method Call
    │
    ▼
HTTP Request (with Auth Token via Interceptor)
    │
    ▼
API Endpoint (Backend)
    │
    ▼
Database Operation
    │
    ▼
API Response
    │
    ▼
Service Returns Observable
    │
    ▼
Component Updates UI
    │
    ▼
User Sees Result
```

## Security Flow

```
User Login
    │
    ▼
API Validates Credentials
    │
    ▼
API Returns JWT Token
    │
    ▼
Frontend Stores Token (localStorage)
    │
    ▼
All Subsequent Requests
    │
    ├─▶ Auth Interceptor Adds Token
    │
    ▼
API Validates Token
    │
    ├─ Valid ──▶ Process Request
    │
    └─ Invalid ──▶ 401 Unauthorized
                   │
                   ▼
              Redirect to Login
```

## Route Guards

```
Route Access Request
    │
    ├─▶ Auth Guard
    │    ├─ Token Exists? ──NO──▶ Redirect to Login
    │    └─ Token Exists? ──YES──▶ Allow Access
    │
    └─▶ Main User Guard
         ├─ Is Main User? ──NO──▶ Redirect to Home
         └─ Is Main User? ──YES──▶ Allow Access
```

## Feature Matrix

| Feature | Component | Service | API Endpoint | Auth Required | Main User Only |
|---------|-----------|---------|--------------|---------------|----------------|
| Check Main User | InitialCheckComponent | AuthService | GET /api/account/is-main-user-required | ❌ | ❌ |
| Register Main User | RegisterMainUserComponent | AuthService | POST /api/account/register-main-user | ❌ | ❌ |
| Login | LoginComponent | AuthService | POST /api/account/login | ❌ | ❌ |
| View Buckets | BucketListComponent | BucketService | GET /api/bucket | ✅ | ❌ |
| Create Bucket | BucketFormComponent | BucketService | POST /api/bucket | ✅ | ❌ |
| Update Bucket | BucketFormComponent | BucketService | PUT /api/bucket/{id} | ✅ | ❌ |
| Delete Bucket | BucketListComponent | BucketService | DELETE /api/bucket/{id} | ✅ | ❌ |
| View Spendings | SpendingListComponent | SpendingService | GET /api/spending | ✅ | ❌ |
| Create Spending | SpendingFormComponent | SpendingService | POST /api/spending | ✅ | ❌ |
| Delete Spending | SpendingListComponent | SpendingService | DELETE /api/spending/{id} | ✅ | ❌ |
| Monthly View | MonthlyViewComponent | MonthlyService | GET /api/monthlybucket/year/{y}/month/{m} | ✅ | ❌ |
| Generate Monthly | MonthlyViewComponent | MonthlyService | POST /api/monthlybucket/generate | ✅ | ❌ |
| View Users | UserListComponent | AuthService | GET /api/account/users | ✅ | ✅ |
| Invite User | UserListComponent | AuthService | POST /api/account/invite-user | ✅ | ✅ |
| Delete User | UserListComponent | AuthService | DELETE /api/account/users/{id} | ✅ | ✅ |

## Technologies Used

```
Frontend Stack
├─ Framework: Angular 18
├─ UI Library: Angular Material
├─ Language: TypeScript
├─ Styling: SCSS
├─ State Management: RxJS Observables
├─ HTTP Client: Angular HttpClient
└─ Build Tool: Angular CLI

Material Components
├─ Navigation: MatSidenav, MatToolbar, MatList
├─ Forms: MatFormField, MatInput, MatSelect, MatDatepicker
├─ Data Display: MatTable, MatCard, MatChips
├─ Dialogs: MatDialog
└─ Feedback: MatSnackBar

Development Tools
├─ Package Manager: npm
├─ Node Version: 20.x
└─ Build Output: dist/zerobudget.ui
```

## File Organization

```
src/zerobudget.ui/
├─ src/
│  ├─ app/
│  │  ├─ components/        (UI Components)
│  │  ├─ services/          (API Integration)
│  │  ├─ models/            (TypeScript Interfaces)
│  │  ├─ guards/            (Route Guards & Interceptors)
│  │  ├─ environments/      (Configuration)
│  │  ├─ app.component.*    (Root Component)
│  │  ├─ app.config.ts      (App Configuration)
│  │  └─ app.routes.ts      (Route Configuration)
│  ├─ index.html            (Entry HTML)
│  ├─ main.ts               (Bootstrap)
│  └─ styles.scss           (Global Styles)
├─ public/                  (Static Assets)
├─ angular.json             (Angular Configuration)
├─ package.json             (Dependencies)
├─ tsconfig.json            (TypeScript Configuration)
└─ README-FRONTEND.md       (Documentation)
```

## Summary

This frontend application provides a complete, production-ready interface for the ZeroBudget API with:

✅ Modern Angular 18 architecture
✅ Material Design UI components
✅ JWT authentication and authorization
✅ Full CRUD operations for all entities
✅ Responsive layout with navigation
✅ Type-safe development
✅ Environment-based configuration
✅ Comprehensive error handling
✅ User-friendly forms and dialogs
✅ Real-time data updates

The application is ready for deployment and can be easily extended with additional features.
