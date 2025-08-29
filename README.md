# Member Rewards API

A comprehensive RESTful API built with .NET 8 for managing customer memberships, reward points, and coupon redemptions.

## 🚀 Features

- **Member Registration & OTP Verification**
- **JWT Token-based Authentication**
- **Points Management System** (₹100 = 10 points)
- **Coupon Redemption System**
- **Responsive Frontend Interface**
- **Complete API Documentation**

## 🛠️ Technology Stack

- **.NET 8** - Web API Framework
- **Entity Framework Core** - ORM
- **In-Memory Database** - For demo purposes
- **JWT Authentication** - Security
- **HTML/CSS/JavaScript** - Frontend

## 📚 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/member/register` | Register new member |
| POST | `/api/member/verify` | Verify OTP and get JWT token |
| POST | `/api/points/add` | Add points to member account |
| GET | `/api/points/{memberId}` | Get member points |
| GET | `/api/points/my-points` | Get current user's points |
| GET | `/api/coupons/my-coupons` | Get available coupons |
| POST | `/api/coupons/redeem` | Redeem coupon |

## 🧪 Testing with Postman

### Import Collection
Click the button below to import the complete Postman collection:

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/33737486-member-rewards-api-collection?action=collection%2Fimport)

Or manually import from URL:
```
https://raw.githubusercontent.com/Ayush-yadav11/MemberRewardsApi/main/MemberRewardsAPI.postman_collection.json
```

### Alternative Import Methods

#### Method 1: Direct GitHub Import
1. Open Postman
2. Click **Import** → **Link**
3. Paste: `https://raw.githubusercontent.com/Ayush-yadav11/MemberRewardsApi/main/MemberRewardsAPI.postman_collection.json`
4. Click **Continue** → **Import**

#### Method 2: Download and Import
1. [Download Collection File](https://raw.githubusercontent.com/Ayush-yadav11/MemberRewardsApi/main/MemberRewardsAPI.postman_collection.json)
2. Open Postman → **Import** → **Upload Files**
3. Select the downloaded JSON file

### Quick Test Flow
1. **Register Member** → Auto-saves Member ID
2. **Verify OTP** (use `123456`) → Auto-saves JWT Token
3. **Add Points** → Test points calculation
4. **View Points** → Check accumulated points
5. **Redeem Coupons** → Test coupon system

## 🏃‍♂️ Running the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/Ayush-yadav11/MemberRewardsApi.git
   cd MemberRewardsApi
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Access the application**
   - Frontend: http://localhost:5113
   - API Documentation: http://localhost:5113/swagger

## 🔑 Demo Credentials

- **OTP**: Always use `123456` for verification
- **Test Mobile**: Any 10-15 digit number (e.g., `9876543210`)

## 📋 Pre-configured Coupons

- **₹50 Off Coupon** - Requires 500 points
- **₹100 Off Coupon** - Requires 1000 points

## 🎯 Testing Workflow

1. Register with mobile number
2. Verify with OTP `123456`
3. Add points by entering purchase amounts
4. Redeem coupons when you have enough points

## 📱 Frontend Features

- Responsive design
- Real-time points calculation
- Automatic OTP prefill
- JWT token management
- Error handling and validation

## 🛡️ Security Features

- JWT token-based authentication
- Input validation and sanitization
- CORS configuration
- Detailed error handling

---

**Built with ❤️ using .NET 8 and Entity Framework Core**