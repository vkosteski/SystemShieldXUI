# 🛡️ SystemShieldX

> A modern, all-in-one system utility suite for Windows  
> Built with 💙 using C# and WPF

---

## ⚡ Overview
![1](https://github.com/user-attachments/assets/828946ea-ab7f-4507-b470-c1dac280da7a)

**SystemShieldX** is a sleek, powerful system maintenance and diagnostic tool designed to optimize, protect, and inform.  
It's a beautifully-designed alternative to tools like CCleaner — but with added muscle and a custom UI engine.

---

## 🚀 Features

### 🦠 Malware Scanner
![4](https://github.com/user-attachments/assets/a456b6ab-4800-4233-a525-dbda6054e40f)

- Quick scan of common malware directories
- SHA-256 based threat detection
- Real-time scan progress + rotating animation
- Threat counter + detailed threat list
- ✅ **Pause / Resume scanning**
- ✅ **Quarantine threats with one click**

---

### 💾 Disk Analyzer
![3](https://github.com/user-attachments/assets/b9907480-de34-44d3-8d38-4ab5be929e73)

- Auto-analyzes your system drive (C:\)
- Shows used, free, and total space
- Health bar with color-coded alerts
- Displays top 10 largest folders
- Built-in ListView with custom styling

---

### 🧹 Optimizer
![2](https://github.com/user-attachments/assets/ef00e7c2-53ca-4214-a4bb-d313f5eb3950)

- **Junk Cleaner**: Scans + deletes temp files
- **System Tweaks**:
  - Clear Recycle Bin
  - Flush DNS Cache
  - Disable Windows Startup Delay
  - Boost System Performance (SysMain & related services)

---

### 🌐 Network Tools
![5](https://github.com/user-attachments/assets/02ee4212-3db2-439f-9e40-d0c012737f7f)

- External IP + location (via `ipapi`)
- Lists active DNS resolvers
- Domain → IP Resolver
- Full **Traceroute** capability (with hop output)

---

### 🖥️ System Info
![6](https://github.com/user-attachments/assets/7490f9ee-34d8-4f9e-9d6e-f04072c17830)

- OS name, architecture
- Machine name
- CPU model + logical cores
- Installed RAM
- GPU model(s)
- System uptime

---

## 🖼️ UI & Design

- Built entirely with **WPF**
- Responsive layouts and modern styling
- Custom blue/dark theme
- Consistent visual language across all tabs
- Sidebar navigation
- Icon-powered branding (with `.ico` support)

---

## 🛠️ Tech Stack

- `C#`, `.NET 6+`, `WPF`
- `System.Management` (WMI)
- `System.Net.Http`, `System.IO`, `Windows APIs`
- `XAML`, `WPF Animations`, `Win32 Interop`

---

## 📦 Installation

1. Clone the repo  
2. Open in Visual Studio (2022+)
3. Build the solution (make sure you're targeting `net6.0-windows` or higher)
4. Run `SystemShieldX.exe`

> Make sure `malware-signatures.json` is included in your output directory!

---

## 📁 Project Structure

<pre> SystemShieldX/ ├── Pages/ │ ├── AntivirusPage.xaml │ ├── OptimizerPage.xaml │ ├── DiskAnalyzerPage.xaml │ ├── NetworkToolsPage.xaml │ └── SystemInfoPage.xaml ├── Assets/ │ ├── scan.png │ ├── image-asset.png │ └── SystemShieldX.ico ├── App.xaml ├── MainWindow.xaml ├── malware-signatures.json └── README.md </pre>

---

## 🔒 Disclaimer

This project is for **educational purposes**.  
SystemShieldX does not upload or distribute virus signatures. You are responsible for the signature list accuracy and any potential false positives.

---

## 🧠 Future Plans

- Quarantine manager (restore/delete)
- Real-time threat monitoring
- Registry cleaner
- Auto-updater integration
- System tray support
