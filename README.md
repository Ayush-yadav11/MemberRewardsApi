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

[![Run in Postman](https://run.pstmn.io/button.svg)](https://raw.githubusercontent.com/YOUR_USERNAME/member-rewards-api/main/MemberRewardsAPI.postman_collection.json)

Or manually import from URL:
```
https://raw.githubusercontent.com/YOUR_USERNAME/member-rewards-api/main/MemberRewardsAPI.postman_collection.json
```

### Quick Test Flow
1. **Register Member** → Auto-saves Member ID
2. **Verify OTP** (use `123456`) → Auto-saves JWT Token
3. **Add Points** → Test points calculation
4. **View Points** → Check accumulated points
5. **Redeem Coupons** → Test coupon system

## 🏃‍♂️ Running the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/member-rewards-api.git
   cd member-rewards-api
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