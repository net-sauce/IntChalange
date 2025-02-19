To run execute docker compose up in main directory
FYI becouse I have diecited to go microsevices route (over engenered).
I had not time to test end to end.
Status
GW
	- at localhost:8000/swagger is avaliable for each service accessible by GW
	- Upload tested
	- Process tested
	- User not tested
BUS
	- Watch tested notification form minio are processed -> bad code in poco classes
	- Status state machine works with perssistent storage in mongodb.
	- Singalr not tested added at last moment to complete whole stack.