import { useState } from "react";

import api from "../Utils/api"; // ← replace axios import
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";

export default function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await api.post(
                "/Auth/login", // ← relative URL
                { Email: email, Password: password }
            );
            if (response.data.success) {
                navigate("/home");
            } else {
                setError("Invalid username or password");
            }
        } catch {
            setError("Invalid username or password");
        }
    };

// ... rest of JSX stays exactly the same

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