# IzSetup - UI/UX Design Document

## 1. Tổng Quan Kiến Trúc UI

IzSetup là một ứng dụng desktop Windows (WPF) với giao diện hiện đại, focus vào tính đơn giản và hiệu quả. Kiến trúc UI tuân theo mô hình **3 màn hình chính** (Three-Step Flow).

---

## 2. Kiến Trúc Màn Hình Chính

### **Mô hình Giao Diện: 3 Bước (Three-Step Flow)**

```
┌─────────────────────────────────────┐
│   STEP 1: SELECT (Chọn Phần Mềm)   │
│   → Danh sách các phần mềm          │
│   → Checkbox để chọn/bỏ chọn        │
│   → Nút "Next" để tiến tới bước 2   │
└─────────────────────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│  STEP 2: REVIEW (Xem Trạng Thái)   │
│   → Tóm tắt các phần mềm đã chọn    │
│   → Thông tin chi tiết (phiên bản)  │
│   → Các nút "Back" & "Install"      │
└─────────────────────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│  STEP 3: INSTALLING (Đang Cài Đặt) │
│   → Progress bar theo phần mềm      │
│   → Real-time logs/status           │
│   → Nút "Cancel" & "Finish"         │
└─────────────────────────────────────┘
```

---

## 3. Chi Tiết Từng Màn Hình

### **SCREEN 1: APPLICATION SELECTION (Chọn Phần Mềm)**

#### **Layout:**

```
┌─────────────────────────────────────────────────────┐
│  IzSetup                                     [─][□][✕] │
├─────────────────────────────────────────────────────┤
│                                                       │
│  📋 STEP 1: SELECT SOFTWARE                         │
│                                                       │
│  Chọn phần mềm bạn muốn cài đặt:                   │
│                                                       │
│  ┌─────────────────────────────────────────────────┐ │
│  │ 🔍 [Search Bar: "Tìm kiếm..."]                │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  ┌─ BROWSERS ─────────────────────────────────────┐ │
│  │ ☐ Google Chrome (v130.0.6723)                  │ │
│  │ ☐ Mozilla Firefox (v132.0)                     │ │
│  │ ☑ Microsoft Edge (v132.0) [Selected]           │ │
│  │ ☐ Opera (v115.0)                               │ │
│  │ ☐ Brave (v1.73.0)                              │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  ┌─ DEVELOPMENT ──────────────────────────────────┐ │
│  │ ☑ Visual Studio Code (v1.96.0) [Selected]      │ │
│  │ ☐ Git (v2.48.0)                                │ │
│  │ ☐ Node.js (v22.0.0)                            │ │
│  │ ☐ Python (v3.12.0)                             │ │
│  │ ☐ Docker (v27.0.0)                             │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  ┌─ UTILITIES ────────────────────────────────────┐ │
│  │ ☐ 7-Zip (v24.05)                               │ │
│  │ ☐ VLC Media Player (v3.0.21)                   │ │
│  │ ☐ Notepad++ (v8.6)                             │ │
│  │ ☐ WinRAR (v7.10)                               │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  Đã chọn: 2 phần mềm                                │
│                                                       │
│  [Back]                                    [Next →]  │
└─────────────────────────────────────────────────────┘
```

#### **Thành Phần Chi Tiết:**

**1. Header Section:**
- Tiêu đề: "STEP 1: SELECT SOFTWARE"
- Mô tả ngắn: "Chọn phần mềm bạn muốn cài đặt:"

**2. Search Bar:**
- Thanh tìm kiếm để filter danh sách phần mềm theo tên
- Hỗ trợ real-time search

**3. Software List (với Grouping):**
- **Nhóm theo Category:** Browsers, Development, Utilities, Multimedia, Productivity, System Tools...
- **Mỗi Item gồm:**
  - Checkbox (chọn/bỏ chọn)
  - Icon phần mềm (16x16 hoặc 24x24)
  - Tên phần mềm
  - Phiên bản hiện có
  - Một dòng mô tả ngắn (optional)

**4. Selected Counter:**
- Hiển thị số lượng phần mềm đã chọn: "Đã chọn: X phần mềm"

**5. Navigation Buttons:**
- **[Back]** - Quay lại (disabled nếu trên màn hình đầu tiên)
- **[Next →]** - Chuyển sang màn hình Review (disabled nếu chưa chọn bất kỳ phần mềm nào)

---

### **SCREEN 2: REVIEW & CONFIRMATION (Xem Trạng Thái)**

#### **Layout:**

```
┌─────────────────────────────────────────────────────┐
│  IzSetup                                     [─][□][✕] │
├─────────────────────────────────────────────────────┤
│                                                       │
│  📋 STEP 2: REVIEW & CONFIRM                       │
│                                                       │
│  Xem lại các phần mềm bạn sắp cài đặt:             │
│                                                       │
│  ┌─────────────────────────────────────────────────┐ │
│  │ 🌐 Google Chrome                                │ │
│  │    Version: v130.0.6723                         │ │
│  │    Size: ~150 MB                                │ │
│  │    Category: Browser                            │ │
│  │    [ℹ️ Details]                                  │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ 🌐 Mozilla Firefox                              │ │
│  │    Version: v132.0                              │ │
│  │    Size: ~200 MB                                │ │
│  │    Category: Browser                            │ │
│  │    [ℹ️ Details]                                  │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ 💻 Visual Studio Code                           │ │
│  │    Version: v1.96.0                             │ │
│  │    Size: ~350 MB                                │ │
│  │    Category: Development                        │ │
│  │    [ℹ️ Details]                                  │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  📊 Summary:                                         │
│  • Total Applications: 3                             │
│  • Estimated Download: ~700 MB                       │
│  • Estimated Time: ~5-10 minutes (tùy tốc độ mạng) │
│                                                       │
│  ☐ Tôi đã đọc các điều khoản sử dụng                │
│                                                       │
│  [← Back]                              [Install →]   │
└─────────────────────────────────────────────────────┘
```

#### **Thành Phần Chi Tiết:**

**1. Header Section:**
- Tiêu đề: "STEP 2: REVIEW & CONFIRM"
- Mô tả: "Xem lại các phần mềm bạn sắp cài đặt:"

**2. Software Summary Cards:**
- **Card Layout cho mỗi phần mềm:**
  - Icon phần mềm (32x32)
  - Tên ứng dụng (bold)
  - Version
  - File size (ước tính)
  - Category
  - Nút "Details" (mở dialog với thông tin chi tiết + homepage link)

**3. Installation Summary:**
- Tổng số ứng dụng
- Tổng dung lượng cần download (ước tính)
- Thời gian dự kiến

**4. Terms Checkbox:**
- ☐ "Tôi đã đọc các điều khoản sử dụng"
- Link đến terms/privacy policy

**5. Navigation Buttons:**
- **[← Back]** - Quay lại màn hình chọn
- **[Install →]** - Bắt đầu cài đặt (disabled nếu checkbox chưa được check)

---

### **SCREEN 3: INSTALLATION PROGRESS (Đang Cài Đặt)**

#### **Layout:**

```
┌─────────────────────────────────────────────────────┐
│  IzSetup                                     [─][□][✕] │
├─────────────────────────────────────────────────────┤
│                                                       │
│  📋 STEP 3: INSTALLING SOFTWARE                    │
│                                                       │
│  Overall Progress:                                   │
│  ████████████████░░░░░░░░░░░░░░░░ 66% (2/3)        │
│                                                       │
│  ┌─────────────────────────────────────────────────┐ │
│  │ ✓ Google Chrome                                 │ │
│  │   ████████████████████████░░░░░░░░░░ 100%       │ │
│  │   Status: ✓ Completed                           │ │
│  │   Time: Completed in 2m 15s                      │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ ⟳ Mozilla Firefox [INSTALLING]                  │ │
│  │   ████████████████░░░░░░░░░░░░░░░░░░░░ 48%      │ │
│  │   Status: ⟳ Downloading... (120 MB / 200 MB)   │ │
│  │   Speed: 12.5 MB/s | ETA: 1m 5s                │ │
│  ├─────────────────────────────────────────────────┤ │
│  │ ⧗ Visual Studio Code [QUEUED]                   │ │
│  │   ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0%     │ │
│  │   Status: ⧗ Queued - Waiting...                 │ │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  📝 Installation Log:                                │
│  ┌─────────────────────────────────────────────────┐ │
│  │ [14:25:32] Starting installation process...     │ │
│  │ [14:25:35] Installing Google Chrome...          │ │
│  │ [14:27:50] ✓ Google Chrome installed            │ │
│  │ [14:27:52] Installing Mozilla Firefox...        │ │
│  │ [14:28:04] Downloading Firefox (120 MB / ...)  │ │
│  │ [14:28:45] Unpacking files...                   │ │
│  │                                        [↓ scroll] │
│  └─────────────────────────────────────────────────┘ │
│                                                       │
│  [Cancel]                                  [Finish]  │
└─────────────────────────────────────────────────────┘
```

#### **Thành Phần Chi Tiết:**

**1. Overall Progress:**
- Progress bar toàn cục (%)
- Hiển thị số lượng: "X/Y"

**2. Individual App Installation Items:**
- **Mỗi item gồm:**
  - Status Icon:
    - ✓ = Completed
    - ⟳ = Installing/In Progress
    - ⧗ = Queued/Waiting
    - ⚠ = Warning/Error
  - Tên ứng dụng
  - Progress bar per app
  - Status text (descriptive):
    - "Downloading... (X MB / Y MB)"
    - "Installing files..."
    - "Finalizing..."
  - Speed info & ETA (nếu đang download)

**3. Real-Time Installation Log:**
- Scrollable text area
- Hiển thị timestamps và messages
- Color coding:
  - Green: ✓ Success
  - Yellow: ⚠ Warning
  - Red: ✗ Error
  - Gray: Info/Debug

**4. Action Buttons:**
- **[Cancel]** - Hủy quá trình cài đặt (nếu đang chạy)
- **[Finish]** - Đóng ứng dụng sau khi hoàn tất

---

## 4. Color Scheme & Design System

### **Color Palette:**

| Color | Hex Value | Usage |
|-------|-----------|-------|
| Primary Blue | `#0078D4` | Buttons, links, active states |
| Dark Gray | `#2D2D2D` | Background, text |
| Light Gray | `#F3F3F3` | Section backgrounds |
| Success Green | `#27AE60` | Completed status |
| Warning Orange | `#F39C12` | Warning/In-progress |
| Error Red | `#E74C3C` | Errors |
| Neutral Gray | `#7F8C8D` | Disabled, secondary text |

### **Typography:**

- **App Title:** Segoe UI, 24px, Bold
- **Screen Title:** Segoe UI, 18px, Bold
- **Section Header:** Segoe UI, 14px, SemiBold
- **Body Text:** Segoe UI, 12px, Regular
- **Small Text (Details):** Segoe UI, 10px, Regular

### **Spacing & Grid:**

- Base unit: 8px
- Margin/Padding: 8px, 16px, 24px (multiples of 8)
- Grid: 12-column layout

---

## 5. Navigation & State Management

### **Screen Flow Diagram:**

```
┌──────────────────┐
│   START          │
└────────┬─────────┘
         │
         ↓
┌──────────────────────────┐
│   SCREEN 1: SELECT       │ ← User selects software
│   (Selection Screen)     │
└────────┬─────────────────┘
         │ [Next] (if selection > 0)
         ↓
┌──────────────────────────┐
│   SCREEN 2: REVIEW       │ ← User reviews & confirms
│   (Confirmation Screen)  │
└────────┬──────────┬──────┘
         │ [Back]   │ [Install] (if terms checked)
         │          │
         ↓          ↓
┌──────────────────────────┐
│   SCREEN 1: SELECT       │   SCREEN 3: INSTALLING
│   (Return)               │   (Progress Screen)
                           └────────┬───────────┘
                                    │ [Cancel] or [Finish]
                                    ↓
                           ┌──────────────────┐
                           │   COMPLETION     │
                           │   (Exit or ...)  │
                           └──────────────────┘
```

### **Screen States:**

| Screen | State | Description |
|--------|-------|-------------|
| Selection | Default | Hiển thị danh sách phần mềm |
| Selection | Search Active | Lọc danh sách theo từ khóa |
| Review | Default | Hiển thị summary & terms |
| Review | Terms Unchecked | Install button disabled |
| Installing | In Progress | Show live progress |
| Installing | Completed | Show finish button |
| Installing | Error/Partial | Show cancel & retry options |

---

## 6. User Experience (UX) Best Practices

### **Key UX Principles:**

1. **Simple 3-Step Flow:**
   - Select → Review → Install
   - Clear visual progression

2. **Feedback & Transparency:**
   - Real-time progress updates
   - Installation logs for troubleshooting
   - Clear status indicators

3. **Control & Cancellation:**
   - Users can go back at any step
   - Cancel installation if needed
   - Clear confirmation prompts

4. **Accessibility:**
   - High contrast colors
   - Keyboard navigation support
   - Clear labels for all controls

5. **Error Handling:**
   - Graceful error messages
   - Retry options for failed installations
   - Detailed logs for debugging

### **Interaction Patterns:**

- **Checkboxes:** Select/deselect software
- **Buttons:** Navigation & actions
- **Progress Bars:** Visual feedback on download/install progress
- **Scrollable Lists:** For large number of apps
- **Category Collapsing:** Optional - collapse/expand app categories

---

## 7. Technical Implementation Details (For Developers)

### **WPF Components:**

**Screen 1 (Selection):**
- `ListBox` or `ItemsControl` with `CheckBox` binding
- `TextBox` for search/filter
- `StackPanel` for category grouping
- Custom `DataTemplate` for app items

**Screen 2 (Review):**
- `ItemsControl` for displaying selected apps
- `TextBlock` for summary stats
- `CheckBox` for terms agreement
- `Hyperlink` for terms/privacy link

**Screen 3 (Progress):**
- `ProgressBar` for overall & per-app progress
- `TextBlock` for status messages
- `ListBox` or `RichTextBox` for logs
- `CancellationTokenSource` for async operations

### **Backend Integration (winget):**

```csharp
// Example: Calling winget
ProcessStartInfo psi = new ProcessStartInfo
{
    FileName = "winget",
    Arguments = $"install --id {appId} --silent --accept-source-agreements",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};
```

---

## 8. Future Enhancements

1. **Advanced Filtering:**
   - Filter by category, size, popularity
   - Sort by name, size, rating

2. **Settings & Preferences:**
   - Installation directory selection
   - Proxy configuration
   - Auto-update preferences

3. **Installation History:**
   - Log of previously installed apps
   - Rollback/uninstall features

4. **Notifications & Alerts:**
   - System notifications on completion
   - Email/Discord notifications (optional)

5. **Multi-Language Support:**
   - Vietnamese, English, Chinese, etc.
   - RTL language support

6. **Custom Software Bundles:**
   - Save/load installation profiles
   - Share bundles with others

---

## 9. Summary

IzSetup UI Design focuses on:
- ✓ **Simplicity:** 3-step flow is intuitive
- ✓ **Clarity:** Real-time feedback & transparency
- ✓ **Control:** Users can navigate, modify, or cancel
- ✓ **Modern Design:** Clean, Windows-native aesthetic
- ✓ **Accessibility:** High contrast, keyboard support

This design ensures users can easily select, review, and install multiple software packages in minutes without leaving the application.
