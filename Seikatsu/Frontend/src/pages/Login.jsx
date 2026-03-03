import { useState } from "react";
import axios from "axios";

function Login() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleLogin = async () => {
        try {
            const response = await axios.post(
                "https://localhost:7115/api/Auth/login",
                {
                    username: username,
                    password: password
                },
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                }
            );

            console.log("Login Success:", response.data);
            localStorage.setItem("token", response.data.accessToken);

        } catch (error) {
            console.error("Login Failed:", error.response?.data);
        }
    };

    return (
        <div>
            <h1>Login</h1>

            <input
                type="text"
                placeholder="Username"
                onChange={(e) => setUsername(e.target.value)}
            />

            <input
                type="password"
                placeholder="Password"
                onChange={(e) => setPassword(e.target.value)}
            />

            <button onClick={handleLogin}>Login</button>
        </div>
    );
}

export default Login;