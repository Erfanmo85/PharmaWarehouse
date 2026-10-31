# PharmaWarehouse

[فارسی](README.fa.md)

A pharmacy inventory (warehouse) management system built with **Angular** and **ASP.NET Core Web API**. It helps a pharmacy keep track of its drugs, categories and batches, and warns about expiring and low-stock items.

## Features

- **Drug management**: add, edit and list drugs
- **Drug categories**: create categories and assign drugs to them
- **Batch management**: add batches for each drug (batch number, purchase price, quantity, expiry date, ...)
- **Expiry tracking**: view batches that are expired or close to expiring
- **Shortage alerts**: see drugs whose stock is running low
- **Inventory overview**: current stock levels per drug and per batch

## Tech Stack

| Layer    | Technology                                   |
|----------|----------------------------------------------|
| Frontend | Angular                                      |
| Backend  | ASP.NET Core Web API (CQRS architecture)     |
| Database | Entity Framework Core                        |

## Project Structure

```
PharmaWarehouse/
├── backend/     # ASP.NET Core Web API
└── frontend/    # Angular application
```

> Adjust the folder names above if yours are different.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (the version used by the project)
- [Node.js](https://nodejs.org/) (LTS) and npm
- Angular CLI: `npm install -g @angular/cli`
- A database server supported by your EF Core configuration (e.g. SQL Server)

## Configuration (Important)

For security reasons, **no passwords, tokens or secrets are stored in this repository**. The values in `appsettings.json` are placeholders. Before running the project, you must provide your own values, such as:

- Database connection string
- JWT / authentication secret key (if used)
- Any other API keys or tokens

Do **not** commit your real secrets. Recommended ways to set them:

**Option 1: .NET User Secrets (recommended for development)**

```bash
cd backend/<YourApiProject>
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_KEY"
```

**Option 2: Local settings file**

Create `appsettings.Development.json` (already in `.gitignore`) and put your own values there.

**Option 3: Environment variables**

```bash
# Linux / macOS
export ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"
```

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Erfanmo85/PharmaWarehouse.git
cd PharmaWarehouse
```

### 2. Run the backend

```bash
cd backend/<YourApiProject>
dotnet restore
dotnet ef database update   # apply migrations
dotnet run
```

The API will start on the address shown in the console (Swagger is usually available at `/swagger`).

### 3. Run the frontend

```bash
cd frontend
npm install
ng serve
```

Open `http://localhost:4200` in your browser.

> Make sure the API URL in the Angular environment file (`src/environments/`) matches the address of your running backend.

## Contributing

1. Fork the project
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -m "Add my feature"`
4. Push the branch: `git push origin feature/my-feature`
5. Open a Pull Request

## License

Add your preferred license here (e.g. MIT).
-------------------------------------------------------------------------------------------------------------
<div dir="rtl">

# PharmaWarehouse (انبارداری داروخانه)

[English](README.md)

سیستم مدیریت انبار داروخانه که با **Angular** و **ASP.NET Core Web API** ساخته شده است. با این برنامه می‌توان داروها، دسته‌بندی‌ها و بچ‌های دارو را مدیریت کرد و از داروهای نزدیک به انقضا یا رو به اتمام مطلع شد.

## قابلیت‌ها

- **مدیریت دارو**: افزودن، ویرایش و نمایش لیست داروها
- **دسته‌بندی دارو**: ساخت دسته و اختصاص دارو به آن
- **مدیریت بچ (Batch)**: افزودن بچ برای هر دارو (شماره بچ، قیمت خرید، تعداد، تاریخ انقضا و ...)
- **پیگیری انقضا**: نمایش بچ‌هایی که منقضی شده‌اند یا به انقضا نزدیک‌اند
- **هشدار کمبود دارو**: نمایش داروهایی که موجودی آن‌ها کم است
- **نمای کلی انبار**: موجودی فعلی هر دارو و هر بچ

## تکنولوژی‌ها

| بخش       | تکنولوژی                                  |
|-----------|-------------------------------------------|
| فرانت‌اند  | Angular                                   |
| بک‌اند     | ASP.NET Core Web API (معماری CQRS)        |
| دیتابیس    | Entity Framework Core                     |

## ساختار پروژه

</div>

```
PharmaWarehouse/
├── backend/     # ASP.NET Core Web API
└── frontend/    # Angular application
```

<div dir="rtl">

> اگر اسم پوشه‌های شما متفاوت است، آن‌ها را اصلاح کنید.

## پیش‌نیازها

- [.NET SDK](https://dotnet.microsoft.com/download) (نسخه‌ای که پروژه استفاده می‌کند)
- [Node.js](https://nodejs.org/) (نسخه LTS) و npm
- Angular CLI: دستور `npm install -g @angular/cli`
- یک سرور دیتابیس سازگار با تنظیمات EF Core (مثلاً SQL Server)

## تنظیمات (مهم)

به دلایل امنیتی، **هیچ رمز، توکن یا اطلاعات حساسی در این ریپازیتوری ذخیره نشده است**. مقادیر داخل `appsettings.json` فقط نمونه هستند. قبل از اجرای پروژه باید مقادیر مخصوص خودتان را وارد کنید، مثل:

- رشته اتصال دیتابیس (Connection String)
- کلید مخفی JWT / احراز هویت (در صورت استفاده)
- هر کلید API یا توکن دیگر

مقادیر واقعی خود را **هرگز commit نکنید**. روش‌های پیشنهادی:

**روش ۱: User Secrets (پیشنهادی برای توسعه)**

</div>

```bash
cd backend/<YourApiProject>
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_KEY"
```

<div dir="rtl">

**روش ۲: فایل تنظیمات محلی**

فایل `appsettings.Development.json` را بسازید (در `.gitignore` قرار دارد) و مقادیر خودتان را آنجا بنویسید.

**روش ۳: متغیرهای محیطی**

</div>

```bash
# Linux / macOS
export ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"
```

<div dir="rtl">

## راه‌اندازی

### ۱. دریافت پروژه

</div>

```bash
git clone https://github.com/Erfanmo85/PharmaWarehouse.git
cd PharmaWarehouse
```

<div dir="rtl">

### ۲. اجرای بک‌اند

</div>

```bash
cd backend/<YourApiProject>
dotnet restore
dotnet ef database update   # اعمال migration ها
dotnet run
```

<div dir="rtl">

API روی آدرسی که در کنسول نمایش داده می‌شود اجرا می‌شود (معمولاً Swagger در مسیر `/swagger` در دسترس است).

### ۳. اجرای فرانت‌اند

</div>

```bash
cd frontend
npm install
ng serve
```

<div dir="rtl">

مرورگر را باز کنید و به آدرس `http://localhost:4200` بروید.

> مطمئن شوید آدرس API در فایل environment انگولار (پوشه `src/environments/`) با آدرس بک‌اند در حال اجرای شما یکی باشد.

## مشارکت در پروژه

1. پروژه را Fork کنید
2. یک branch جدید بسازید: `git checkout -b feature/my-feature`
3. تغییرات را commit کنید: `git commit -m "Add my feature"`
4. branch را push کنید: `git push origin feature/my-feature`
5. یک Pull Request باز کنید

## لایسنس

لایسنس دلخواه خود را اینجا اضافه کنید (مثلاً MIT).

</div>
