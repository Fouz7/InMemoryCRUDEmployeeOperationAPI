# InMemoryCRUDEmployeeOperationDhiki

## Package yang Digunakan
- `Microsoft.AspNetCore.OpenApi` 8.0.7
- `Microsoft.EntityFrameworkCore` 8.0.8
- `Microsoft.EntityFrameworkCore.InMemory` 8.0.8
- `Microsoft.EntityFrameworkCore.Tools` 8.0.8
- `Swashbuckle.AspNetCore` 6.4.0


## Cara Menjalankan Aplikasi
### 1. Install Dependencies:  
Pastikan Anda telah menginstal .NET SDK.
Buka terminal di direktori proyek dan jalankan perintah berikut untuk mengembalikan dependensi proyek:
```bash
dotnet restore
```

### 2. Build Proyek:  
Jalankan perintah berikut untuk membangun proyek:
```bash
dotnet build
```

### 3. Jalankan Aplikasi:  
Jalankan perintah berikut untuk memulai aplikasi:
```bash
dotnet run
```
### 4. Akses Aplikasi:
Buka browser web dan navigasikan ke https://localhost:5006/swagger untuk mengakses Swagger UI dan berinteraksi dengan endpoint API.

### 5 Akses secara live:
Buka browser web dan navigasikan ke https://inmemorycrudemployeeoperation-cqhag0arf5h2dfeb.eastus2-01.azurewebsites.net/swagger/index.html untuk mengakses Swagger UI dan berinteraksi dengan endpoint API.

### Format body request
{
"employeeId": "1001",
"fullName": "Adit",
"birthDate": "17-Aug-1954"
}