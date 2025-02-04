# HNG12 Stage 1 - Public API
  This is a simple public API developed for the HNG12 Internship Stage 1 task.

## Number Classification API

This API classifies numbers based on their mathematical properties and returns a fun fact.

### 🚀 Features:
- Checks if a number is **prime, perfect, or Armstrong**.
- Determines if it is **odd/even**.
- Calculates the **digit sum**.
- Fetches a **fun fact** from the Numbers API.

### 📌 Endpoint:
#### `GET /api/classify-number?number=371`
Example Response:
```json
{
    "number": 371,
    "is_prime": false,
    "is_perfect": false,
    "properties": ["armstrong", "odd"],
    "digit_sum": 11,
    "fun_fact": "371 is an Armstrong number because 3^3 + 7^3 + 1^3 = 371"
}






# HNG12 Stage 0 - Public API

This is a simple public API developed for the HNG12 Internship Stage 0 task.

## 🚀 API Endpoint
Base URL: `https://hng-task.onrender.com`

### 📌 `GET /api/info`
This endpoint returns basic information about the developer and the project.

**Response:**
```json
{
  "email": "yasiroyebo@gmail.com",
  "current_datetime": "2025-02-03T16:17:18.8736560Z",
  "github_url": "https://github.com/YastecHub/HNG_Tasks"
}
