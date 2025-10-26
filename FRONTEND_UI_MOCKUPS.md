# ZeroBudget Frontend - UI Mockups

## Login Screen

```
┌─────────────────────────────────────────────────────┐
│                                                     │
│                                                     │
│          ┌───────────────────────────┐             │
│          │  Login to ZeroBudget      │             │
│          ├───────────────────────────┤             │
│          │                           │             │
│          │  Email                    │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ your@email.com      │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          │  Password                 │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ ••••••••            │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          │  ┌─────────────────────┐  │             │
│          │  │      Login          │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          └───────────────────────────┘             │
│                                                     │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## Main Application Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ ZeroBudget                               user@email.com    [Logout] │
├─────────────────────────────────────────────────────────────────────┤
│                │                                                     │
│   ☰ Navigation │                 Main Content Area                  │
│                │                                                     │
│   📅 Monthly   │  ┌──────────────────────────────────────────────┐  │
│                │  │                                              │  │
│   📦 Buckets   │  │                                              │  │
│                │  │                                              │  │
│   💰 Spendings │  │            Component Content                │  │
│                │  │                                              │  │
│   ➕ New       │  │                                              │  │
│   Spending     │  │                                              │  │
│                │  │                                              │  │
│   👥 Users*    │  │                                              │  │
│                │  │                                              │  │
│                │  └──────────────────────────────────────────────┘  │
│                │                                                     │
└─────────────────────────────────────────────────────────────────────┘
* Main User Only
```

## Buckets List

```
┌─────────────────────────────────────────────────────────────────────┐
│  Buckets                                                            │
│  Manage your budget categories                                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  [➕ Create Bucket]                                                 │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Name        │ Description    │ Default Limit │ Status │ Actions│  │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ Groceries   │ Weekly food    │ $500.00      │ ✓ Enabled│✏️🗑️│   │
│  │ Transport   │ Gas & transit  │ $200.00      │ ✓ Enabled│✏️🗑️│   │
│  │ Entertainment│ Fun activities│ $150.00      │ ✓ Enabled│✏️🗑️│   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Create/Edit Bucket Dialog

```
                ┌───────────────────────────┐
                │  Create Bucket            │
                ├───────────────────────────┤
                │                           │
                │  Name                     │
                │  ┌─────────────────────┐  │
                │  │ Groceries           │  │
                │  └─────────────────────┘  │
                │                           │
                │  Description              │
                │  ┌─────────────────────┐  │
                │  │ Weekly groceries    │  │
                │  │ and food items      │  │
                │  └─────────────────────┘  │
                │                           │
                │  Default Limit            │
                │  ┌─────────────────────┐  │
                │  │ 500.00              │  │
                │  └─────────────────────┘  │
                │                           │
                │         [Cancel] [Create] │
                └───────────────────────────┘
```

## New Spending Form

```
┌─────────────────────────────────────────────────────────────────────┐
│  New Spending                                                       │
│  Add a new expense                                                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Bucket                                                             │
│  ┌─────────────────────────────────────────┐                       │
│  │ Groceries                          ▼    │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│  Date                                                               │
│  ┌─────────────────────────────────────────┐ 📅                    │
│  │ 2025-10-26                              │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│  Description                                                        │
│  ┌─────────────────────────────────────────┐                       │
│  │ Weekly grocery shopping                 │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│  Amount                                                             │
│  ┌─────────────────────────────────────────┐                       │
│  │ 125.50                                  │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│  Owner                                                              │
│  ┌─────────────────────────────────────────┐                       │
│  │ John Doe                                │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│  Tags                                                               │
│  ┌─────────────────────────────────────────┐                       │
│  │ [food ×] [weekly ×] [essential ×]       │                       │
│  │ Add tag...                              │                       │
│  └─────────────────────────────────────────┘                       │
│                                                                     │
│                               [Cancel] [Create Spending]            │
└─────────────────────────────────────────────────────────────────────┘
```

## Monthly View

```
┌─────────────────────────────────────────────────────────────────────┐
│  Monthly View                                                       │
│  Budget overview for the selected month                             │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  [October ▼]  [2025 ▼]  [Generate Monthly Data]                    │
│                                                                     │
│  Monthly Buckets                                                    │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Bucket      │ Limit     │ Spent     │ Remaining              │  │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ Groceries   │ $500.00   │ $425.50   │ $74.50 (green)        │  │
│  │ Transport   │ $200.00   │ $215.00   │ -$15.00 (red)         │  │
│  │Entertainment│ $150.00   │ $89.00    │ $61.00 (green)        │  │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Monthly Spendings                                                  │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Date       │ Description        │ Amount    │ Owner          │  │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ 2025-10-26 │ Weekly groceries   │ $125.50   │ John Doe      │  │
│  │ 2025-10-25 │ Gas station        │ $65.00    │ Jane Smith    │  │
│  │ 2025-10-24 │ Movie tickets      │ $45.00    │ John Doe      │  │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## User Management (Main User Only)

```
┌─────────────────────────────────────────────────────────────────────┐
│  User Management                                                    │
│  Manage invited users (Main User Only)                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Invite New User                                                    │
│  ┌─────────────────────────────────┐                               │
│  │ newuser@example.com             │  [📧 Send Invitation]          │
│  └─────────────────────────────────┘                               │
│                                                                     │
│  Active Users                                                       │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Email              │ Role          │ Actions                 │  │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ admin@budget.com   │ [Main User]   │ 🗑️ (disabled)           │  │
│  │ user1@budget.com   │ User          │ 🗑️                      │  │
│  │ user2@budget.com   │ User          │ 🗑️                      │  │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Pending Invitations                                                │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Email              │ Expires At    │ Status   │ Token        │  │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ pending@email.com  │ Nov 2, 2025   │ Pending  │ abc123def... │  │
│  │ used@email.com     │ Oct 20, 2025  │ Used     │ xyz789ghi... │  │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Register Main User

```
┌─────────────────────────────────────────────────────┐
│                                                     │
│                                                     │
│          ┌───────────────────────────┐             │
│          │  Register Main User       │             │
│          │  Create the primary       │             │
│          │  administrator account    │             │
│          ├───────────────────────────┤             │
│          │                           │             │
│          │  Email                    │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ admin@zerobudget.com│  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          │  Password                 │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ ••••••••            │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          │  Confirm Password         │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ ••••••••            │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          │  ┌─────────────────────┐  │             │
│          │  │ Register Main User  │  │             │
│          │  └─────────────────────┘  │             │
│          │                           │             │
│          └───────────────────────────┘             │
│                                                     │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## Color Scheme & Theme

```
Primary Color:   Purple (#673AB7)
Accent Color:    Green (#4CAF50)
Background:      White (#FFFFFF)
Text:            Dark Gray (#212121)
Secondary Text:  Light Gray (#757575)

Status Indicators:
✓ Enabled:       Green
✗ Disabled:      Red
⚠️ Over Budget:  Red
✓ Under Budget:  Green
```

## Responsive Behavior

### Desktop (> 768px)
- Side navigation always visible
- Full-width tables
- Multi-column layouts

### Tablet (768px - 1024px)
- Side navigation collapsible
- Responsive tables
- Adjusted spacing

### Mobile (< 768px)
- Side navigation as drawer
- Stacked cards instead of tables
- Touch-friendly buttons

## Interactions

### Buttons
- Primary: Raised button with primary color
- Secondary: Flat button
- Icon: Icon button for actions

### Forms
- Outlined form fields
- Real-time validation
- Error messages below fields
- Submit disabled when invalid

### Tables
- Sortable columns (where applicable)
- Hover effects on rows
- Action buttons per row
- Responsive design

### Dialogs
- Modal overlays
- Backdrop click to close (with confirmation)
- ESC key to close
- Focus management

## Material Design Elevation

```
Level 0: Background
Level 1: Cards
Level 2: Raised buttons (on hover)
Level 3: Dialogs
Level 4: Toolbar
```

## Icons Used

- 🏠 Home
- 📅 Calendar/Monthly
- 📦 Category/Bucket
- 💰 Money/Spending
- ➕ Add/Create
- ✏️ Edit
- 🗑️ Delete
- 👥 Users/People
- 📧 Email/Invite
- 🚪 Logout
- ✓ Success/Enabled
- ✗ Error/Disabled
- ⚙️ Settings

## Summary

The frontend provides a clean, intuitive interface following Material Design principles:

✓ Consistent color scheme (Purple/Green)
✓ Clear visual hierarchy
✓ Responsive across devices
✓ Touch-friendly interactions
✓ Accessible form controls
✓ Real-time validation feedback
✓ Loading states
✓ Error messages
✓ Success confirmations
✓ Intuitive navigation
