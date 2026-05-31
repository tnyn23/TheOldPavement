# The Old Pavement - E-commerce Platform

Nền tảng thương mại điện tử cho thương hiệu thời trang streetwear The Old Pavement.

## 🚀 Công nghệ sử dụng

- **Backend**: ASP.NET Core 9.0 (Razor Pages)
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core với Pomelo.EntityFrameworkCore.MySql
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Web)
- **Authentication**: Cookie-based Authentication
- **Payment**: MoMo Payment Gateway
- **Email**: SMTP (Gmail)
- **Containerization**: Docker

## 📋 Yêu cầu hệ thống

- .NET 9.0 SDK
- MySQL 8.0+
- Docker Desktop (optional)

## ⚙️ Cài đặt

### 1. Clone repository

```bash
git clone <repository-url>
cd TheOldPavement
```

### 2. Cấu hình Database

Tạo file `src/Web/appsettings.Development.json` từ template:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=the_old_pavement;User Id=root;Password=your_password;CharSet=utf8mb4;"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  },
  "Momo": {
    "PartnerCode": "YOUR_MOMO_PARTNER_CODE",
    "AccessKey": "YOUR_MOMO_ACCESS_KEY",
    "SecretKey": "YOUR_MOMO_SECRET_KEY",
    "ApiUrl": "https://test-payment.momo.vn/v2/gateway/api/create"
  }
}
```

### 3. Import Database

```bash
mysql -u root -p the_old_pavement < database/schema.sql
```

### 4. Chạy ứng dụng

#### Chạy trực tiếp với .NET CLI

```bash
cd src/Web
dotnet run
```

Truy cập: `http://localhost:5081`

#### Chạy với Docker

```bash
# Build và publish
dotnet publish src/Web/Web.csproj -c Release -o publish

# Build Docker image
docker build -t theoldpavement .

# Run container
docker run -d -p 5081:8080 --name theoldpavement-app theoldpavement
```

Truy cập: `http://localhost:5081`

## 🎨 Thiết kế

### Brand Colors

- **Forest Green**: `#3E503C` - Primary brand color
- **Sage**: `#7F886A` - Secondary, muted text
- **Cream**: `#F3ECDB` - Background, light elements
- **Orange**: `#FF6F3D` - Accent, CTAs

### Design Principles

- Premium streetwear aesthetic
- Minimal, clean layouts
- Uppercase typography with wide tracking
- High contrast for readability

## 📁 Cấu trúc dự án

```
TheOldPavement/
├── src/
│   ├── Domain/              # Entities, Enums
│   ├── Application/         # Services, Interfaces, DTOs
│   ├── Infrastructure/      # DbContext, Repositories
│   └── Web/                 # Razor Pages, wwwroot
├── Dockerfile
├── PROJECT_OVERVIEW.md
└── README.md
```

## 🔐 Bảo mật

- **KHÔNG** commit file `appsettings.Development.json` hoặc `appsettings.Production.json`
- **KHÔNG** commit file backup database (*.sql, *.bak)
- Sử dụng App Password cho Gmail SMTP
- Lưu trữ secrets trong environment variables khi deploy production

## 🚢 Deploy

### Railway (Recommended)

1. Tạo MySQL database trên Railway
2. Cập nhật connection string trong Railway environment variables
3. Deploy từ GitHub repository

### Docker

```bash
# Build image
docker build -t theoldpavement .

# Run với environment variables
docker run -d \
  -p 5081:8080 \
  -e ConnectionStrings__DefaultConnection="Server=your-db;..." \
  --name theoldpavement-app \
  theoldpavement
```

## 📝 Features

- ✅ Product catalog với filtering và search
- ✅ Shopping cart và wishlist
- ✅ User authentication (Cookie-based)
- ✅ Checkout với multiple payment methods
- ✅ MoMo payment integration
- ✅ Email notifications
- ✅ Admin dashboard (coming soon)
- ✅ Order management
- ✅ Responsive design

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

This project is private and proprietary.

## 👥 Team

- **Developer**: [Your Name]
- **Designer**: [Designer Name]
- **Project**: EXE201 - FPT University

## 📞 Contact

- Email: theoldpavement@gmail.com
- Website: [Coming Soon]
