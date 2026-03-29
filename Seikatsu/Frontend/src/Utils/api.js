import axios from "axios";

const api = axios.create({
	baseURL: "/api",
	withCredentials: true, // sends cookies with every request
});

export default api;