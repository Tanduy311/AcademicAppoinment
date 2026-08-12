## 🌿 Quy tắc làm việc với Git

### Tạo branch mới trước khi code

```bash
git checkout -b feature/ten-chuc-nang
```

### Commit thường xuyên

```bash
git add .
git commit -m "Add: mô tả ngắn chức năng"
git push origin feature/ten-chuc-nang
```

### Tạo Pull Request để merge vào main

Vào GitHub → **Pull Requests** → **New Pull Request** → chọn branch của bạn → nhờ người khác review trước khi merge.

### Quy tắc đặt tên branch

| Loại | Ví dụ |
|------|-------|
| Tính năng mới | `feature/login` |
| Sửa lỗi | `fix/loi-dang-nhap` |
| Database | `db/them-bang-orders` |

---

## Frontend

Frontend nằm trong thư mục `frontend/`.

Chạy local:

```bash
cd frontend
npm install
npm run dev
```

Nếu backend chạy ở port khác, set thêm file `.env`:

```bash
VITE_API_BASE_URL=http://localhost:xxxx
```
