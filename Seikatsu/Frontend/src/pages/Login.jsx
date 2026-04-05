import { useState } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";
import { Button } from "@/components/ui/button"; 

export default function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(
                "/api/Auth/login",
                { Email: email, Password: password }
            );
            if (response.data.success) {
                navigate("/home", { replace: true });
            } else {
                setError("Invalid username or password");
            }
        } catch (err) {
            if (err.response?.status === 409) {
                navigate("/home", { replace: true });
            } else {
                setError("Invalid username or password");
            }
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-left">
                <h2 className="text-4xl md:text-5xl font-extrabold tracking-tight text-black mb-4 drop-shadow-[0_2px_8px_rgba(0,0,0,0.6)]">
                    Welcome <span className="text-red-500">Back!</span>
                </h2>
                <p className="text-black/100 text-xl leading-relaxed mb-10 drop-shadow-[0_1px_4px_rgba(0,0,0,0.5)]">
                    Continue your journey with Seikatsu.Discover international products tailored for life in Japan.
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
                    <Button variant="outline" className="px-110 py-110 text-lg text-white rounded-xl border-gray-300 bg-rose-500
    shadow-[0_6px_0_rgba(0,0,0,0.2)]
    hover:shadow-[0_10px_20px_rgba(0,0,0,0.2)]
    hover:-translate-y-1
    active:translate-y-1 active:shadow-[0_2px_0_rgba(0,0,0,0.2)]
    transition-all duration-1500">
                        Login
                    </Button>
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