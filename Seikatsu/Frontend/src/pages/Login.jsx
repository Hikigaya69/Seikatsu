
import { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";

export default function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [checking, setChecking] = useState(true); // ← checking auth on load

    // ← Guest guard — runs once on page load
    useEffect(() => {
        axios.get("/api/Auth/check", {
            withCredentials: true
        }) 
            .then(res => {
                // Axios stores the HTTP status code in 'res.status'
                if (res.status === 200) {
                    navigate("/home", { replace: true });
                }
            })
            .catch(() => {
               //
            })
            .finally(() => {
                setChecking(false);
            });
    }, [navigate]); // Added navigate to the dependency array

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(
                "/api/Auth/login",
                { Email: email, Password: password }
            );
            if (response.data.success) {
                navigate("/home", { replace: true }); // ← replace, not just navigate
            } else {
                setError("Invalid username or password");
            }
        } catch (err) {
            if (err.response?.status === 409) {
                navigate("/home", { replace: true }); // ← already logged in safety net
            } else {
                setError("Invalid username or password");
            }
        }
    };

    // ← Don't flash the login form while checking
    if (checking) return null; // or a spinner if you prefer

    return (
        <div className="auth-container">
            <div className="auth-left">
                <h1>Welcome Back</h1>
                <p>
                    Continue your journey with Seikatsu.
                    Discover international groceries tailored for life in Japan.
                </p>
            </div>
            <div className="auth-right">
                <form className="auth-card" onSubmit={handleLogin}>
                    <h2>Login</h2>
                    {error && <p className="error">{error}</p>}
                    <input
                        type="text"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                    <button type="submit">Login</button>
                    <div className="auth-link">
                        Don't have an account?{" "}
                        <Link to="/register">
                            <span>Create one</span>
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    );
}