# Microservices Project Documentation

To run the application, execute the following command in the main directory:

```
docker compose up
```

## Note
I decided to go the microservices route (potentially over-engineered), and due to time constraints, I wasn't able to test the system end-to-end.

---

## Current Status

### Gateway (GW)
- Available at **[localhost:8000/swagger](http://localhost:8000/swagger)** for each service accessible through the Gateway.
- **Upload**: Tested ✅
- **Process**: Tested ✅
- **User**: Not tested ❌

### Message Bus (BUS)
- **Watch**: Tested ✅ - Notifications from MinIO are processed successfully; however, there is **bad code in POCO classes** that needs to be fixed.
- **Status State Machine**: Verified to work with persistent storage in MongoDB ✅
- **SignalR**: Not tested ❌ - Added at the last moment to complete the stack.

---


