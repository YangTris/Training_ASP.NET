# API Documentation Index

This directory contains comprehensive API documentation for frontend developers working with the MockTest e-commerce API.

## 📚 Documentation Files

### 1. [FRONTEND_GUIDE.md](./FRONTEND_GUIDE.md) ⭐ **START HERE**

Quick start guide for frontend developers. Includes:

- 5-minute quick start
- Authentication flow examples
- Common use cases with code
- React component examples
- Error handling patterns
- Troubleshooting common issues

**Best for:** New developers who want to get started quickly

---

### 2. [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)

Complete API reference documentation. Includes:

- All endpoints with full details
- Request/response examples for every endpoint
- Authentication and authorization rules
- Error codes and formats
- JavaScript/Fetch examples
- React/Axios examples
- Enum values and data types

**Best for:** Looking up specific endpoint details, request/response formats

---

### 3. [API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md)

Quick lookup table for developers. Includes:

- Endpoint summary tables
- Common workflow patterns
- Quick code snippets
- Enum reference
- Tips and reminders

**Best for:** Quick lookups while coding, refreshing memory on endpoints

---

### 4. [MockTest_API.postman_collection.json](./MockTest_API.postman_collection.json)

Postman/Thunder Client collection. Includes:

- All API endpoints pre-configured
- Environment variables (auto-set from responses)
- Example request bodies
- Test scripts for auto-saving tokens

**Best for:** Testing endpoints, exploring the API, debugging issues

---

## 🚀 Quick Navigation

### I want to...

**...get started quickly**
→ [FRONTEND_GUIDE.md](./FRONTEND_GUIDE.md) - Read the 5-minute quick start

**...understand authentication**
→ [API_DOCUMENTATION.md - Authentication section](./API_DOCUMENTATION.md#authentication)

**...see all product endpoints**
→ [API_DOCUMENTATION.md - Products section](./API_DOCUMENTATION.md#products)

**...implement cart functionality**
→ [API_DOCUMENTATION.md - Cart section](./API_DOCUMENTATION.md#cart)

**...create an order flow**
→ [API_QUICK_REFERENCE.md - User Registration & First Purchase](./API_QUICK_REFERENCE.md#user-registration--first-purchase)

**...test endpoints manually**
→ Import [MockTest_API.postman_collection.json](./MockTest_API.postman_collection.json) into Postman

**...see request/response examples**
→ [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) - Every endpoint has examples

**...handle errors properly**
→ [API_DOCUMENTATION.md - Error Handling section](./API_DOCUMENTATION.md#error-handling)

**...look up an endpoint quickly**
→ [API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md) - Tables with all endpoints

---

## 📋 API Overview

### Base URL

```
http://localhost:5296/api
```

### Available Resources

- **Authentication** - Register, login, JWT tokens
- **Products** - CRUD operations, pagination, search, images
- **Categories** - Category management (admin-protected)
- **Cart** - Shopping cart management (user-specific)
- **Orders** - Order creation and management
- **Users** - User management

### Authentication

- **Type:** JWT Bearer tokens
- **Lifetime:** 30 minutes
- **Header format:** `Authorization: Bearer <token>`
- **Test accounts:**
  - User: `user@example.com` / `P@ssw0rd`
  - Admin: `admin@example.com` / `P@ssw0rd`

---

## 🎯 Common Use Cases

### 1. Browse Products (No Auth)

```javascript
GET /api/product?pageNumber=1&pageSize=10
```

### 2. User Login

```javascript
POST /api/auth/login
Body: { "email": "user@example.com", "password": "P@ssw0rd" }
```

### 3. Add to Cart (Authenticated)

```javascript
POST /api/cart/items
Headers: Authorization: Bearer <token>
Body: { "productId": "guid", "quantity": 2 }
```

### 4. Create Order (Authenticated)

```javascript
POST /api/order
Headers: Authorization: Bearer <token>
Body: { "shippingAddress": "...", "paymentMethod": 0 }
```

### 5. Admin: Update Order Status

```javascript
PATCH /api/order/{id}/status
Headers: Authorization: Bearer <admin-token>
Body: { "status": 2 }
```

---

## 🔑 Key Features

### Product Images

- **List view** (`GET /product`): Returns `mainImageUrl` for each product
- **Detail view** (`GET /product/{id}`): Returns full `images` array with all product images
- **New products**: Automatically get a default image

### Pagination

- All list endpoints support pagination
- Parameters: `pageNumber`, `pageSize`, `searchTerm`, `sortBy`, `isDescending`
- Response includes: `items`, `totalItems`, `pageNumber`, `pageSize`, `totalPages`

### Soft Deletes

- Products and categories are soft-deleted (marked `IsDeleted=true`)
- Deleted items don't appear in queries
- Data is preserved in the database

### Cart Behavior

- One cart per user (automatically created on first access)
- Cart is cleared after creating an order
- Cart items include product snapshot (name, price, image)

### Authorization

- Public: Products, Categories (read), Auth
- User: Cart, Orders (own orders only)
- Admin: Category management, order status updates, view all orders

---

## 📊 Response Formats

### Successful Responses

- `200 OK` - Request succeeded (with body)
- `201 Created` - Resource created (with Location header)
- `204 No Content` - Request succeeded (no body)

### Error Responses

All errors return:

```json
{
  "message": "Descriptive error message"
}
```

Status codes:

- `400` - Bad request (validation error)
- `401` - Unauthorized (missing/invalid token)
- `403` - Forbidden (insufficient permissions)
- `404` - Not found
- `409` - Conflict (duplicate email, etc.)
- `500` - Server error

---

## 🛠️ Tools & Testing

### Postman Collection

1. Import `MockTest_API.postman_collection.json`
2. Run "Auth > Login" to get a token
3. Token is automatically saved and used for authenticated requests
4. Variables like `productId`, `orderId` are auto-set from responses

### Manual Testing with cURL

```bash
# Login
curl -X POST http://localhost:5296/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"P@ssw0rd"}'

# Get products
curl http://localhost:5296/api/product?pageNumber=1&pageSize=10

# Get cart (with token)
curl http://localhost:5296/api/cart \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Browser DevTools

- Network tab to inspect requests/responses
- Console to test fetch calls
- Application/Storage tab to view saved tokens

---

## 💡 Best Practices

1. **Store tokens securely** - Use localStorage/sessionStorage, never expose in URLs
2. **Handle token expiration** - Redirect to login on 401 responses
3. **Validate input** - Check data before sending to API
4. **Show loading states** - Provide feedback during API calls
5. **Handle errors gracefully** - Display user-friendly messages
6. **Use pagination** - Don't load all items at once
7. **Cache when appropriate** - Product lists, categories can be cached briefly
8. **Use HTTPS in production** - Protect tokens and data

---

## 🔄 Typical Workflows

### New User Shopping Flow

1. Browse products (no auth) → `GET /api/product`
2. Register → `POST /api/auth/register`
3. Login → `POST /api/auth/login` (save token)
4. Add items to cart → `POST /api/cart/items`
5. View cart → `GET /api/cart`
6. Checkout → `POST /api/order`
7. View order history → `GET /api/order`

### Admin Product Management

1. Login as admin → `POST /api/auth/login`
2. Create category → `POST /api/category`
3. Create product → `POST /api/product`
4. View all orders → `GET /api/order/all`
5. Update order status → `PATCH /api/order/{id}/status`

---

## 🆘 Troubleshooting

### API not responding

- Ensure API is running: `cd API; dotnet run`
- Check URL: `http://localhost:5296/api`
- Verify port in terminal output

### 401 Unauthorized

- Token expired (30 min) → Login again
- Token not in header → Check Authorization header format
- Token invalid → Verify "Bearer " prefix

### 404 Not Found

- Wrong endpoint → Check documentation
- Resource doesn't exist → Verify ID
- Typo in URL → Double-check spelling

### CORS errors

- API should handle CORS by default
- If issues persist, check API/Program.cs CORS configuration

---

## 📞 Support & Resources

### Documentation

- **Full reference:** [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
- **Quick lookup:** [API_QUICK_REFERENCE.md](./API_QUICK_REFERENCE.md)
- **Getting started:** [FRONTEND_GUIDE.md](./FRONTEND_GUIDE.md)

### Testing

- **Postman collection:** [MockTest_API.postman_collection.json](./MockTest_API.postman_collection.json)

### Architecture

- **Backend guide:** [.github/copilot-instructions.md](./.github/copilot-instructions.md)

### Contact

For questions or issues, contact the backend development team.

---

## 📝 Version Information

- **API Version:** 1.0
- **Last Updated:** October 30, 2025
- **Base Framework:** ASP.NET Core (.NET 9.0)
- **Authentication:** JWT Bearer tokens
- **Database:** SQL Server (Entity Framework Core)

---

## 🎓 Learning Path

### Beginner

1. Read [FRONTEND_GUIDE.md](./FRONTEND_GUIDE.md) quick start
2. Test with Postman collection
3. Try the example React component
4. Implement login + product list

### Intermediate

1. Implement full authentication flow
2. Add cart functionality
3. Create order checkout flow
4. Handle errors and loading states

### Advanced

1. Implement admin features
2. Add optimistic updates
3. Implement token refresh
4. Add offline support with caching
5. Set up automated testing

---

**Ready to start? Begin with [FRONTEND_GUIDE.md](./FRONTEND_GUIDE.md)! 🚀**
