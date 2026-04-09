import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";
import api from "../Utils/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Eye, EyeOff } from "lucide-react";

export default function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [showPassword, setShowPassword] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await api.post("/Auth/login", {
                Email: email,
                Password: password,
            });
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
                    Continue your journey with Seikatsu. Discover international
                    products tailored for life in Japan.
                </p>
            </div>

            <div className="auth-right">
                <form className="auth-card" onSubmit={handleLogin}>
                    <h2>Login</h2>
                    {error && <p className="error">{error}</p>}

                    {/* Email */}
                    <input
                        type="text"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />

                    <div className="relative w-full px-10 py-10">
                        <Input
                            className="bg-background pr-10"
                            id="password-toggle"
                            placeholder="Enter your password"
                            type={showPassword ? "text" : "password"}
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                        <button
                            className="absolute inset-y-0 right-2 flex items-center justify-center text-muted-foreground hover:text-foreground"
                            onClick={() => setShowPassword(!showPassword)}
                            type="button"
                        >
                            {showPassword ? (
                                <EyeOff className="h-4 w-4" />
                            ) : (
                                <Eye className="h-4 w-4" />
                            )}
                        </button>
                    </div>

                    {/* Forgot password */}
                    <div className="w-full text-right text-sm">
                        <Link
                            to="/forgot-password"
                            className="text-[#284b63] hover:underline"
                        >
                            Forgot Password?
                        </Link>
                    </div>

                    {/* Login button */}
                    <Button
                        type="submit"
                        className="w-full text-lg text-white rounded-xl border-gray-300 bg-rose-500
                            shadow-[0_6px_0_rgba(0,0,0,0.2)]
                            hover:shadow-[0_10px_20px_rgba(0,0,0,0.2)]
                            hover:-translate-y-1
                            active:translate-y-1 active:shadow-[0_2px_0_rgba(0,0,0,0.2)]
                            transition-all duration-150"
                    >
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