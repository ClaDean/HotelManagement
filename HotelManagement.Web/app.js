// API基础地址
const API_BASE = 'http://localhost:5091/api';

// 页面切换
document.querySelectorAll('.nav-link').forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();

        // 移除所有active类
        document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
        e.target.classList.add('active');

        // 隐藏所有页面
        document.querySelectorAll('.page-content').forEach(p => p.style.display = 'none');

        // 显示对应页面
        const page = e.target.dataset.page;
        document.getElementById(page + '-page').style.display = 'block';

        // 加载数据
        loadPageData(page);
    });
});

// 加载页面数据
function loadPageData(page) {
    switch (page) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'rooms':
            loadRooms();
            break;
        case 'locks':
            loadLocks();
            break;
        case 'bookings':
            loadBookings();
            break;
        case 'records':
            loadRecords();
            break;
    }
}

// 加载仪表盘
async function loadDashboard() {
    try {
        const [rooms, locks, bookings] = await Promise.all([
            fetch(`${API_BASE}/Rooms`).then(r => r.json()),
            fetch(`${API_BASE}/DoorLocks`).then(r => r.json()),
            fetch(`${API_BASE}/Bookings`).then(r => r.json())
        ]);

        document.getElementById('total-rooms').textContent = rooms.length;
        document.getElementById('available-rooms').textContent =
            rooms.filter(r => r.status === 'Available').length;
        document.getElementById('total-locks').textContent = locks.length;

        const today = new Date().toISOString().split('T')[0];
        document.getElementById('today-checkins').textContent =
            bookings.filter(b => b.actualCheckInTime?.startsWith(today)).length;
    } catch (error) {
        console.error('加载仪表盘失败:', error);
        alert('加载数据失败，请确保后端API正在运行');
    }
}

// 加载房间列表
async function loadRooms() {
    try {
        const response = await fetch(`${API_BASE}/Rooms`);
        const rooms = await response.json();

        const tbody = document.getElementById('rooms-table-body');
        tbody.innerHTML = '';

        rooms.forEach(room => {
            const statusColor = {
                'Available': 'success',
                'Occupied': 'danger',
                'Reserved': 'warning',
                'Maintenance': 'secondary'
            }[room.status] || 'secondary';

            const statusText = {
                'Available': '空闲',
                'Occupied': '已占用',
                'Reserved': '已预订',
                'Maintenance': '维护中'
            }[room.status] || room.status;

            tbody.innerHTML += `
                <tr>
                    <td>${room.roomNumber}</td>
                    <td>${room.roomType}</td>
                    <td>${room.floor}楼</td>
                    <td><span class="badge bg-${statusColor} status-badge">${statusText}</span></td>
                    <td>¥${room.price}</td>
                    <td>${room.doorLockId ? '已绑定' : '<span class="text-muted">未绑定</span>'}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteRoom(${room.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        });
    } catch (error) {
        console.error('加载房间失败:', error);
    }
}

// 加载门锁列表
async function loadLocks() {
    try {
        const response = await fetch(`${API_BASE}/DoorLocks`);
        const locks = await response.json();

        const tbody = document.getElementById('locks-table-body');
        tbody.innerHTML = '';

        locks.forEach(lock => {
            const statusColor = lock.status === 'Online' ? 'success' : 'secondary';
            const statusText = lock.status === 'Online' ? '在线' : '离线';

            tbody.innerHTML += `
                <tr>
                    <td>${lock.deviceId}</td>
                    <td>${lock.deviceName}</td>
                    <td>${lock.manufacturer}</td>
                    <td>${lock.model}</td>
                    <td>${lock.roomId ? '房间' + lock.roomId : '<span class="text-muted">未绑定</span>'}</td>
                    <td><span class="badge bg-${statusColor}">${statusText}</span></td>
                    <td>${lock.batteryLevel ? lock.batteryLevel + '%' : '-'}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary" onclick="unlockDoor(${lock.id})">
                            <i class="bi bi-unlock"></i> 远程开锁
                        </button>
                    </td>
                </tr>
            `;
        });
    } catch (error) {
        console.error('加载门锁失败:', error);
    }
}

// 加载订单列表
async function loadBookings() {
    try {
        const response = await fetch(`${API_BASE}/Bookings`);
        const bookings = await response.json();

        const tbody = document.getElementById('bookings-table-body');
        tbody.innerHTML = '';

        bookings.forEach(booking => {
            const statusColor = {
                'Pending': 'warning',
                'Confirmed': 'info',
                'CheckedIn': 'success',
                'CheckedOut': 'secondary',
                'Cancelled': 'danger'
            }[booking.status] || 'secondary';

            const statusText = {
                'Pending': '待确认',
                'Confirmed': '已确认',
                'CheckedIn': '已入住',
                'CheckedOut': '已退房',
                'Cancelled': '已取消'
            }[booking.status] || booking.status;

            tbody.innerHTML += `
                <tr>
                    <td>#${booking.id}</td>
                    <td>${booking.room?.roomNumber || '-'}</td>
                    <td>${booking.guestName}</td>
                    <td>${booking.guestPhone}</td>
                    <td>${new Date(booking.checkInTime).toLocaleString('zh-CN')}</td>
                    <td>${new Date(booking.checkOutTime).toLocaleString('zh-CN')}</td>
                    <td><span class="badge bg-${statusColor} status-badge">${statusText}</span></td>
                    <td>¥${booking.totalPrice}</td>
                    <td>
                        ${booking.status === 'Pending' ?
                    `<button class="btn btn-sm btn-success" onclick="checkIn(${booking.id})">
                                <i class="bi bi-box-arrow-in-right"></i> 入住
                            </button>` : ''}
                        ${booking.status === 'CheckedIn' ?
                    `<button class="btn btn-sm btn-warning" onclick="checkOut(${booking.id})">
                                <i class="bi bi-box-arrow-right"></i> 退房
                            </button>` : ''}
                    </td>
                </tr>
            `;
        });
    } catch (error) {
        console.error('加载订单失败:', error);
    }
}

// 加载开锁记录
async function loadRecords() {
    try {
        // 获取所有门锁的记录
        const locksResponse = await fetch(`${API_BASE}/DoorLocks`);
        const locks = await locksResponse.json();

        const tbody = document.getElementById('records-table-body');
        tbody.innerHTML = '';

        // 暂时显示示例数据（因为需要每个门锁的记录接口）
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">暂无开锁记录</td></tr>';
    } catch (error) {
        console.error('加载记录失败:', error);
    }
}

// 显示添加房间模态框
function showAddRoomModal() {
    const modal = new bootstrap.Modal(document.getElementById('addRoomModal'));
    modal.show();
}

// 添加房间
async function addRoom() {
    const data = {
        roomNumber: document.getElementById('roomNumber').value,
        roomType: document.getElementById('roomType').value,
        floor: parseInt(document.getElementById('floor').value),
        status: 'Available',
        price: parseFloat(document.getElementById('price').value),
        description: document.getElementById('description').value
    };

    try {
        const response = await fetch(`${API_BASE}/Rooms`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('addRoomModal')).hide();
            document.getElementById('addRoomForm').reset();
            loadRooms();
            alert('添加成功！');
        } else {
            alert('添加失败！');
        }
    } catch (error) {
        console.error('添加房间失败:', error);
        alert('添加失败！');
    }
}

// 删除房间
async function deleteRoom(id) {
    if (!confirm('确定要删除这个房间吗？')) return;

    try {
        const response = await fetch(`${API_BASE}/Rooms/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            loadRooms();
            alert('删除成功！');
        } else {
            alert('删除失败！');
        }
    } catch (error) {
        console.error('删除房间失败:', error);
        alert('删除失败！');
    }
}

// 显示添加门锁模态框
async function showAddLockModal() {
    // 加载可用房间列表
    const response = await fetch(`${API_BASE}/Rooms`);
    const rooms = await response.json();

    const select = document.getElementById('lockRoomId');
    select.innerHTML = '<option value="">暂不绑定</option>';
    rooms.forEach(room => {
        select.innerHTML += `<option value="${room.id}">${room.roomNumber} - ${room.roomType}</option>`;
    });

    const modal = new bootstrap.Modal(document.getElementById('addLockModal'));
    modal.show();
}

// 添加门锁
async function addLock() {
    const roomId = document.getElementById('lockRoomId').value;
    const data = {
        deviceId: document.getElementById('deviceId').value,
        deviceName: document.getElementById('deviceName').value,
        manufacturer: document.getElementById('manufacturer').value,
        model: document.getElementById('model').value,
        roomId: roomId ? parseInt(roomId) : null
    };

    try {
        const response = await fetch(`${API_BASE}/DoorLocks`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('addLockModal')).hide();
            document.getElementById('addLockForm').reset();
            loadLocks();
            alert('添加成功！');
        } else {
            alert('添加失败！');
        }
    } catch (error) {
        console.error('添加门锁失败:', error);
        alert('添加失败！');
    }
}

// 远程开锁
async function unlockDoor(lockId) {
    if (!confirm('确定要远程开锁吗？')) return;

    try {
        const response = await fetch(`${API_BASE}/DoorLocks/${lockId}/unlock`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: 1 })
        });

        if (response.ok) {
            alert('开锁指令已发送！');
        } else {
            alert('开锁失败！');
        }
    } catch (error) {
        console.error('远程开锁失败:', error);
        alert('开锁失败！');
    }
}

// 显示添加订单模态框
async function showAddBookingModal() {
    // 加载可用房间列表
    const response = await fetch(`${API_BASE}/Rooms`);
    const rooms = await response.json();
    const availableRooms = rooms.filter(r => r.status === 'Available');

    const select = document.getElementById('bookingRoomId');
    select.innerHTML = '';
    availableRooms.forEach(room => {
        select.innerHTML += `<option value="${room.id}">${room.roomNumber} - ${room.roomType} (¥${room.price})</option>`;
    });

    // 设置默认时间
    const now = new Date();
    const tomorrow = new Date(now.getTime() + 24 * 60 * 60 * 1000);
    document.getElementById('checkInTime').value = formatDateTimeLocal(now);
    document.getElementById('checkOutTime').value = formatDateTimeLocal(tomorrow);

    const modal = new bootstrap.Modal(document.getElementById('addBookingModal'));
    modal.show();
}

// 格式化日期时间为 datetime-local 格式
function formatDateTimeLocal(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

// 添加订单
async function addBooking() {
    const data = {
        roomId: parseInt(document.getElementById('bookingRoomId').value),
        guestName: document.getElementById('guestName').value,
        guestPhone: document.getElementById('guestPhone').value,
        guestIdCard: document.getElementById('guestIdCard').value,
        checkInTime: document.getElementById('checkInTime').value,
        checkOutTime: document.getElementById('checkOutTime').value,
        totalPrice: parseFloat(document.getElementById('totalPrice').value),
        paidAmount: 0
    };

    try {
        const response = await fetch(`${API_BASE}/Bookings`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('addBookingModal')).hide();
            document.getElementById('addBookingForm').reset();
            loadBookings();
            alert('订单创建成功！');
        } else {
            const error = await response.json();
            alert('创建失败：' + (error.message || '未知错误'));
        }
    } catch (error) {
        console.error('添加订单失败:', error);
        alert('创建失败！');
    }
}

// 办理入住
async function checkIn(bookingId) {
    if (!confirm('确定要办理入住吗？')) return;

    try {
        const response = await fetch(`${API_BASE}/Bookings/${bookingId}/checkin`, {
            method: 'POST'
        });

        if (response.ok) {
            const result = await response.json();

            // 提取密码
            if (result.booking && result.booking.temporaryPasswords && result.booking.temporaryPasswords.length > 0) {
                const password = result.booking.temporaryPasswords[0].password;

                // 显示密码模态框
                document.getElementById('tempPassword').textContent = password;
                const modal = new bootstrap.Modal(document.getElementById('passwordModal'));
                modal.show();
            } else {
                alert('入住成功！但未生成密码（可能房间未绑定门锁）');
            }

            loadBookings();
            loadDashboard();
        } else {
            const error = await response.json();
            alert('入住失败：' + (error.message || '未知错误'));
        }
    } catch (error) {
        console.error('办理入住失败:', error);
        alert('入住失败！');
    }
}

// 办理退房
async function checkOut(bookingId) {
    if (!confirm('确定要办理退房吗？')) return;

    try {
        const response = await fetch(`${API_BASE}/Bookings/${bookingId}/checkout`, {
            method: 'POST'
        });

        if (response.ok) {
            loadBookings();
            loadDashboard();
            alert('退房成功！');
        } else {
            const error = await response.json();
            alert('退房失败：' + (error.message || '未知错误'));
        }
    } catch (error) {
        console.error('办理退房失败:', error);
        alert('退房失败！');
    }
}

// 页面加载完成后初始化
document.addEventListener('DOMContentLoaded', () => {
    loadDashboard();
});
