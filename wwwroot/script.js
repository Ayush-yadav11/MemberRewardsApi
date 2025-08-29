// Configuration
const API_BASE_URL = 'http://localhost:5113/api'; // Update this to match your API URL
let authToken = localStorage.getItem('authToken');
let currentMemberId = localStorage.getItem('memberId');

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    setupEventListeners();
    
    // Check if user is already authenticated
    if (authToken && currentMemberId) {
        showDashboard();
        refreshPoints();
        refreshCoupons();
    }
});

function setupEventListeners() {
    // Registration form
    document.getElementById('registerForm').addEventListener('submit', handleRegistration);
    
    // OTP verification form
    document.getElementById('verifyForm').addEventListener('submit', handleOTPVerification);
    
    // Add points form
    document.getElementById('addPointsForm').addEventListener('submit', handleAddPoints);
}

// Tab functionality
function showTab(tabName, event) {
    // Hide all tab contents
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.remove('active');
    });
    
    // Remove active class from all tab buttons
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    
    // Show selected tab
    document.getElementById(tabName).classList.add('active');
    
    // Add active class to clicked button
    if (event && event.target) {
        event.target.classList.add('active');
    } else {
        // Fallback: find and activate the button by data attribute or text content
        document.querySelectorAll('.tab-btn').forEach(btn => {
            if (btn.textContent.toLowerCase().includes(tabName.toLowerCase())) {
                btn.classList.add('active');
            }
        });
    }
}

function showDashboardTab(tabName, event) {
    // Hide all dashboard tab contents
    document.querySelectorAll('#dashboardSection .tab-content').forEach(content => {
        content.classList.remove('active');
    });
    
    // Remove active class from all dashboard tab buttons
    document.querySelectorAll('.dashboard-tabs .tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    
    // Show selected tab
    document.getElementById(tabName).classList.add('active');
    
    // Add active class to clicked button
    if (event && event.target) {
        event.target.classList.add('active');
    } else {
        // Fallback: find and activate the button by text content
        document.querySelectorAll('.dashboard-tabs .tab-btn').forEach(btn => {
            if (btn.textContent.toLowerCase().includes(tabName.toLowerCase()) || 
                (tabName === 'points' && btn.textContent.includes('My Points')) ||
                (tabName === 'addPoints' && btn.textContent.includes('Add Points')) ||
                (tabName === 'coupons' && btn.textContent.includes('Coupons'))) {
                btn.classList.add('active');
            }
        });
    }
}

// Authentication functions
async function handleRegistration(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const requestData = {
        mobileNumber: formData.get('mobileNumber'),
        name: formData.get('name') || null,
        email: formData.get('email') || null
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/member/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });
        
        const result = await response.json();
        
        if (result.success) {
            showMessage(`Registration successful! OTP: ${result.otp}`, 'success');
            
            // Pre-fill the verification form
            document.getElementById('verifyMobile').value = requestData.mobileNumber;
            document.getElementById('otp').value = '123456'; // Auto-fill dummy OTP
            showTab('verify', null);
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('Registration failed: ' + error.message, 'error');
    }
}

async function handleOTPVerification(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const requestData = {
        mobileNumber: formData.get('verifyMobile'),
        otp: formData.get('otp')
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/member/verify`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });
        
        const result = await response.json();
        
        if (result.success) {
            // Store authentication data
            authToken = result.token;
            currentMemberId = result.memberId;
            localStorage.setItem('authToken', authToken);
            localStorage.setItem('memberId', currentMemberId);
            
            showMessage('OTP verified successfully!', 'success');
            showDashboard();
            refreshPoints();
            refreshCoupons();
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('OTP verification failed: ' + error.message, 'error');
    }
}

// Dashboard functions
function showDashboard() {
    document.getElementById('authSection').style.display = 'none';
    document.getElementById('dashboardSection').style.display = 'block';
    
    // Update user info
    document.getElementById('userInfo').textContent = `Member ID: ${currentMemberId}`;
    
    // Pre-fill member ID in add points form
    document.getElementById('memberId').value = currentMemberId;
}

async function handleAddPoints(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const requestData = {
        memberId: parseInt(formData.get('memberId')),
        purchaseAmount: parseFloat(formData.get('purchaseAmount')),
        description: formData.get('description') || null
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/points/add`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(requestData)
        });
        
        const result = await response.json();
        
        if (result.success) {
            showMessage(`Points added successfully! Earned: ${result.pointsEarned} points`, 'success');
            
            // Reset form
            event.target.reset();
            document.getElementById('memberId').value = currentMemberId;
            
            // Refresh points display
            refreshPoints();
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('Failed to add points: ' + error.message, 'error');
    }
}

async function refreshPoints() {
    try {
        const response = await fetch(`${API_BASE_URL}/points/my-points`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            // Update total points display
            document.getElementById('totalPoints').textContent = result.totalPoints;
            
            // Update transaction history
            const transactionList = document.getElementById('transactionList');
            transactionList.innerHTML = '';
            
            if (result.transactions.length === 0) {
                transactionList.innerHTML = '<p>No transactions found.</p>';
            } else {
                result.transactions.forEach(transaction => {
                    const transactionElement = document.createElement('div');
                    transactionElement.className = 'transaction-item';
                    transactionElement.innerHTML = `
                        <div class="transaction-date">${new Date(transaction.date).toLocaleDateString()}</div>
                        <div class="transaction-details">
                            <span>${transaction.description || 'Purchase'}</span>
                            <span><strong>₹${transaction.purchaseAmount} → ${transaction.pointsEarned} points</strong></span>
                        </div>
                    `;
                    transactionList.appendChild(transactionElement);
                });
            }
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('Failed to refresh points: ' + error.message, 'error');
    }
}

async function refreshCoupons() {
    try {
        const response = await fetch(`${API_BASE_URL}/coupons/my-coupons`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const couponsList = document.getElementById('couponsList');
            couponsList.innerHTML = '';
            
            if (result.coupons.length === 0) {
                couponsList.innerHTML = '<p>No coupons available.</p>';
            } else {
                result.coupons.forEach(coupon => {
                    const couponElement = document.createElement('div');
                    couponElement.className = `coupon-item ${coupon.canRedeem ? 'redeemable' : 'not-redeemable'}`;
                    couponElement.innerHTML = `
                        <div class="coupon-header">
                            <span class="coupon-name">${coupon.name}</span>
                            <span class="coupon-value">₹${coupon.couponValue}</span>
                        </div>
                        <div class="coupon-details">${coupon.description || 'No description'}</div>
                        <div class="coupon-points">Points required: ${coupon.pointsRequired}</div>
                        <button class="redeem-btn" 
                                onclick="redeemCoupon(${coupon.id})" 
                                ${!coupon.canRedeem ? 'disabled' : ''}>
                            ${coupon.canRedeem ? 'Redeem' : 'Insufficient Points'}
                        </button>
                    `;
                    couponsList.appendChild(couponElement);
                });
            }
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('Failed to refresh coupons: ' + error.message, 'error');
    }
}

async function redeemCoupon(couponId) {
    try {
        const response = await fetch(`${API_BASE_URL}/coupons/redeem/${couponId}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            showMessage(`Coupon redeemed successfully! Code: ${result.couponCode}`, 'success');
            
            // Refresh both points and coupons
            refreshPoints();
            refreshCoupons();
        } else {
            showMessage(result.message, 'error');
        }
    } catch (error) {
        showMessage('Failed to redeem coupon: ' + error.message, 'error');
    }
}

function logout() {
    authToken = null;
    currentMemberId = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('memberId');
    
    // Reset forms
    document.getElementById('registerForm').reset();
    document.getElementById('verifyForm').reset();
    document.getElementById('addPointsForm').reset();
    
    // Show auth section
    document.getElementById('authSection').style.display = 'block';
    document.getElementById('dashboardSection').style.display = 'none';
    
    // Reset to register tab
    showTab('register', null);
    
    showMessage('Logged out successfully!', 'info');
}

// Utility functions
function showMessage(message, type) {
    const messageElement = document.getElementById('message');
    messageElement.textContent = message;
    messageElement.className = `message ${type}`;
    messageElement.classList.add('show');
    
    // Hide message after 5 seconds
    setTimeout(() => {
        messageElement.classList.remove('show');
    }, 5000);
}