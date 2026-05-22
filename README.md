<div align="center">
  <h1>🎓 School Management & E-Learning Application</h1>
  <p>A comprehensive, feature-rich web platform for modern online education.</p>

  ![.NET Core](https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
  ![SQL Server](https://img.shields.io/badge/SQL_Server-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
  ![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)
  ![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=for-the-badge&logo=microsoft&logoColor=white)
</div>

<br />

## 📖 Overview
This project is an advanced **School Management and E-Learning Application** designed to bridge the gap between traditional administrative systems and interactive online learning. It connects **Administrators**, **Instructors**, and **Trainees** in a unified, real-time, and AI-powered educational ecosystem.

---

## ✨ Key Features

### 👨‍🏫 For Instructors & Admins
- **Course & Department Management:** Neatly organize educational content into specific departments.
- **AI-Powered Quizzes:** Utilize **Google Gemini AI** to automatically generate relevant quiz questions and save hours of manual work.
- **Attendance Tracking:** Easily record and monitor trainee attendance for active sessions.
- **Global Announcements:** Broadcast important news (term starts, holidays, etc.) to all users instantly.

### 👩‍🎓 For Trainees (Students)
- **Interactive Dashboard:** Track course progress and manage enrolled materials.
- **Real-Time Live Chat:** Communicate instantly with peers and instructors using **SignalR**.
- **Automated Certifications:** Receive dynamically generated, downloadable PDF certificates (via **QuestPDF**) upon course completion.
- **Leaderboards:** A gamified experience that ranks top-performing trainees based on their quiz scores.
- **Seamless Enrollment:** Securely enroll in premium courses using the integrated **Paymob** payment gateway.

### ⚙️ System Capabilities
- **Google OAuth Login:** Quick and secure third-party authentication.
- **Live Notifications:** Real-time alerts without page reloads.
- **Bilingual Support (i18n):** Full support for both **English (en-US)** and **Arabic (ar-EG)**.
- **Data Export:** Export reports and attendance to Excel using **ClosedXML**.

---

## 🛠️ Architecture & Technology Stack

**Backend:**
*   ASP.NET Core MVC (.NET 8.0)
*   Entity Framework Core (SQL Server)
*   ASP.NET Core Identity (Role-Based Access Control)
*   SignalR (Real-time Hubs for Chat & Notifications)

**Integrations:**
*   **Google Gemini API:** Intelligent Quiz Generation.
*   **Paymob API:** Secure Payment Processing.
*   **Google Authentication:** OAuth 2.0 Sign-in.

**Libraries & Tools:**
*   **QuestPDF:** Automated Certificate Generation.
*   **ClosedXML:** Excel Spreadsheet Exporting.
*   **Bootstrap 5:** Responsive UI Design.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 / VS Code

### Installation
1. **Clone the repository:**
   ```bash
   git clone https://github.com/hassanomara842/SchoolApp.git
   cd SchoolApp
   ```

2. **Configure User Secrets (Recommended) or `appsettings.json`:**
   You will need to provide your own API keys for the third-party integrations to work:
   ```json
   "GeminiSettings": {
     "ApiKey": "YOUR_GEMINI_API_KEY"
   },
   "Authentication": {
     "Google": {
       "ClientId": "YOUR_GOOGLE_CLIENT_ID",
       "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
     }
   },
   "PaymobSettings": {
     "ApiKey": "YOUR_PAYMOB_API_KEY",
     "HMACSecret": "YOUR_PAYMOB_HMAC_SECRET",
     "IntegrationId": "YOUR_PAYMOB_INTEGRATION_ID",
     "IframeId": "YOUR_PAYMOB_IFRAME_ID"
   }
   ```

3. **Apply Database Migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

---

<div align="center">
  <i>Developed with ❤️ for Modern Education.</i>
</div>