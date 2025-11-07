# Product Image Upload Guide

## Overview

Products now require at least one image when created. Images are uploaded to Supabase Storage and stored with the product.

## Configuration

### 1. Update `appsettings.json`

Add your Supabase configuration:

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "ServiceKey": "YOUR_SUPABASE_SERVICE_KEY",
    "BucketName": "Training_img"
  }
}
```

**Important:** Replace `YOUR_SUPABASE_SERVICE_KEY` with your actual Supabase service role key (found in Supabase Dashboard → Settings → API → service_role key).

### 2. Ensure Supabase Bucket Exists

1. Go to Supabase Dashboard → Storage
2. Create a bucket named `Training_img` (or use your configured name)
3. Set bucket to **Public** if you want images publicly accessible
4. Configure allowed file types: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`

## API Changes

### Create Product Endpoint

**Before (JSON):**
```http
POST /api/product
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Description",
  "price": 99.99,
  "categoryId": "guid"
}
```

**After (Multipart Form Data):**
```http
POST /api/product
Content-Type: multipart/form-data

name: Product Name
description: Description
price: 99.99
categoryId: guid
images: [file1.jpg, file2.jpg]  // At least 1 required, Index of the main image (0-based)
```

## Image Requirements

- **Format:** `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
- **Size:** Maximum 5 MB per file
- **Quantity:** At least 1 image required
- **Main Image:** The first image is main

## Frontend Integration

### JavaScript/Fetch Example

```javascript
async function createProduct(productData, imageFiles) {
  const formData = new FormData();
  
  // Add product fields
  formData.append('name', productData.name);
  formData.append('description', productData.description);
  formData.append('price', productData.price);
  formData.append('categoryId', productData.categoryId);
  formData.append('mainImageIndex', 0); // First image as main
  
  // Add image files
  imageFiles.forEach((file) => {
    formData.append('images', file);
  });
  
  const response = await fetch('http://localhost:5296/api/product', {
    method: 'POST',
    body: formData
    // Note: Don't set Content-Type header, browser will set it automatically with boundary
  });
  
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }
  
  return await response.json();
}

// Usage
const files = document.getElementById('imageInput').files;
const fileArray = Array.from(files);

await createProduct({
  name: 'Gaming Laptop',
  description: 'High performance laptop',
  price: 1299.99,
  categoryId: 'category-guid'
}, fileArray);
```

### React Example

```jsx
import { useState } from 'react';

function CreateProductForm() {
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    categoryId: '',
  });
  const [images, setImages] = useState([]);

  const handleImageChange = (e) => {
    setImages(Array.from(e.target.files));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (images.length === 0) {
      alert('Please select at least one image');
      return;
    }

    const formPayload = new FormData();
    formPayload.append('name', formData.name);
    formPayload.append('description', formData.description);
    formPayload.append('price', formData.price);
    formPayload.append('categoryId', formData.categoryId);
    formPayload.append('mainImageIndex', 0);

    images.forEach((file) => {
      formPayload.append('images', file);
    });

    try {
      const response = await fetch('http://localhost:5296/api/product', {
        method: 'POST',
        body: formPayload,
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message);
      }

      const product = await response.json();
      console.log('Product created:', product);
      alert('Product created successfully!');
    } catch (error) {
      console.error('Error:', error);
      alert(error.message);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Name"
        value={formData.name}
        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
        required
      />
      <textarea
        placeholder="Description"
        value={formData.description}
        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
      />
      <input
        type="number"
        step="0.01"
        placeholder="Price"
        value={formData.price}
        onChange={(e) => setFormData({ ...formData, price: e.target.value })}
        required
      />
      <input
        type="text"
        placeholder="Category ID"
        value={formData.categoryId}
        onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
        required
      />
      <input
        type="file"
        accept="image/*"
        multiple
        onChange={handleImageChange}
        required
      />
      <p>{images.length} image(s) selected</p>
      <button type="submit">Create Product</button>
    </form>
  );
}
```

### Axios Example

```javascript
import axios from 'axios';

async function createProduct(productData, imageFiles) {
  const formData = new FormData();
  
  formData.append('name', productData.name);
  formData.append('description', productData.description);
  formData.append('price', productData.price);
  formData.append('categoryId', productData.categoryId);
  formData.append('mainImageIndex', 0);
  
  imageFiles.forEach((file) => {
    formData.append('images', file);
  });
  
  const response = await axios.post(
    'http://localhost:5296/api/product',
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  );
  
  return response.data;
}
```

## Postman/Thunder Client Testing

### Using Postman

1. Create a new request: `POST http://localhost:5296/api/product`
2. Go to **Body** tab
3. Select **form-data**
4. Add fields:
   - `name` (Text): "Test Product"
   - `description` (Text): "Test description"
   - `price` (Text): "99.99"
   - `categoryId` (Text): "your-category-guid"
   - `mainImageIndex` (Text): "0"
   - `images` (File): Select one or more image files
5. Send the request

### Using cURL

```bash
curl -X POST http://localhost:5296/api/product \
  -F "name=Test Product" \
  -F "description=Test description" \
  -F "price=99.99" \
  -F "categoryId=your-category-guid" \
  -F "mainImageIndex=0" \
  -F "images=@/path/to/image1.jpg" \
  -F "images=@/path/to/image2.jpg"
```

## Response Format

### Success Response (201 Created)

```json
{
  "id": "product-guid",
  "name": "Test Product",
  "description": "Test description",
  "price": 99.99,
  "categoryId": "category-guid",
  "createdAt": "2025-11-04T10:30:00Z",
  "updatedAt": "2025-11-04T10:30:00Z",
  "mainImageUrl": "https://your-project.supabase.co/storage/v1/object/public/Training_img/products/guid1.jpg",
  "images": [
    {
      "id": "image-guid-1",
      "imageUrl": "https://your-project.supabase.co/storage/v1/object/public/Training_img/products/guid1.jpg",
      "isMain": true
    },
    {
      "id": "image-guid-2",
      "imageUrl": "https://your-project.supabase.co/storage/v1/object/public/Training_img/products/guid2.jpg",
      "isMain": false
    }
  ]
}
```

## Error Responses

### No Images Provided (400 Bad Request)
```json
{
  "message": "At least one product image is required"
}
```

### Invalid Image Format (400 Bad Request)
```json
{
  "message": "Invalid image file 'document.pdf'. Allowed formats: .jpg, .jpeg, .png, .gif, .webp. Max size: 5MB"
}
```

### Invalid Main Image Index (400 Bad Request)
```json
{
  "message": "Main image index must be between 0 and 2"
}
```

### File Too Large (400 Bad Request)
```json
{
  "message": "Invalid image file. Allowed formats: .jpg, .jpeg, .png, .gif, .webp. Max size: 5MB"
}
```

### Upload Failed (500 Internal Server Error)
```json
{
  "message": "Failed to upload product images: [error details]"
}
```

## Validation Rules

1. **At least one image required** - Cannot create product without images
2. **Valid image formats only** - .jpg, .jpeg, .png, .gif, .webp
3. **File size limit** - Maximum 5 MB per file
4. **Main image index** - Must be within valid range (0 to images.length - 1)
5. **Category must exist** - CategoryId must reference an existing category

## HTML Form Example

```html
<!DOCTYPE html>
<html>
<head>
  <title>Create Product</title>
</head>
<body>
  <h1>Create Product</h1>
  <form id="productForm">
    <div>
      <label>Name:</label>
      <input type="text" name="name" required>
    </div>
    <div>
      <label>Description:</label>
      <textarea name="description"></textarea>
    </div>
    <div>
      <label>Price:</label>
      <input type="number" step="0.01" name="price" required>
    </div>
    <div>
      <label>Category ID:</label>
      <input type="text" name="categoryId" required>
    </div>
    <div>
      <label>Images (at least 1):</label>
      <input type="file" name="images" accept="image/*" multiple required>
    </div>
    <div>
      <label>Main Image Index:</label>
      <input type="number" name="mainImageIndex" value="0" min="0">
    </div>
    <button type="submit">Create Product</button>
  </form>

  <script>
    document.getElementById('productForm').addEventListener('submit', async (e) => {
      e.preventDefault();
      
      const formData = new FormData(e.target);
      
      try {
        const response = await fetch('http://localhost:5296/api/product', {
          method: 'POST',
          body: formData
        });
        
        if (!response.ok) {
          const error = await response.json();
          alert('Error: ' + error.message);
          return;
        }
        
        const product = await response.json();
        alert('Product created successfully! ID: ' + product.id);
        console.log('Product:', product);
        e.target.reset();
      } catch (error) {
        alert('Error: ' + error.message);
      }
    });
  </script>
</body>
</html>
```

## Troubleshooting

### "Supabase URL not configured"
- Ensure `Supabase:Url` is set in `appsettings.json`

### "Supabase Key not configured"
- Ensure `Supabase:ServiceKey` is set in `appsettings.json`
- Use the **service_role** key, not the anon key

### "Failed to upload file to Supabase"
- Check Supabase bucket exists and is configured correctly
- Verify service key has permissions
- Check Supabase bucket is public or has correct policies

### "Invalid image file"
- Check file format (must be image)
- Check file size (max 5MB)
- Verify file extension is allowed

### Images not appearing
- Check Supabase bucket is set to public
- Verify bucket name matches configuration
- Check CORS settings in Supabase

## Best Practices

1. **Validate on frontend** - Check file size and format before upload
2. **Show preview** - Display image previews before submission
3. **Progress indicator** - Show upload progress for better UX
4. **Error handling** - Display clear error messages
5. **Image optimization** - Consider resizing images before upload
6. **Multiple images** - Allow users to select multiple images at once
7. **Drag and drop** - Implement drag-and-drop for better UX

## Notes

- Images are stored in Supabase Storage under `products/` folder
- Each image gets a unique GUID filename
- Image URLs are public and can be accessed directly
- First image (index 0) is the main image by default
- Main image appears in product list views
- All images appear in product detail view

---

**Last Updated:** November 4, 2025
