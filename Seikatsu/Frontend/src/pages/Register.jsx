import { useState } from "react";
import api from "../Utils/api";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";
 import { Button } from "@/components/ui/button";

export default function Register() {
    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [email, setEmail] = useState("");
    const [error, setError] = useState("");

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      await api.post(
          "/Auth/register",
          { FullName: username, Password: password, Email: email }
      );
        navigate("/login", { replace: true });
    } catch {
      setError("User already exists");
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-left">
              <h2 className="text-4xl md:text-5xl font-extrabold tracking-tight text-black mb-4 drop-shadow-[0_2px_8px_rgba(0,0,0,0.6)]">
                  Create <span className="text-red-500">Account</span>
              </h2>
              <p className="text-black/100 text-xl leading-relaxed mb-10 drop-shadow-[0_1px_4px_rgba(0,0,0,0.5)]">
                  Join Seikatsu and start exploring authentic
                  international groceries across Japan.
              </p>
      </div>

      <div className="auth-right">
              <form className="auth-card" onSubmit={handleRegister}>
                  <h2 className="text-lg text-red-600 ">Register</h2>

          {error && <p className="error">{error}</p>}

          <input
            type="text"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
                  />
                  <input
                      type="text"
                      placeholder="email@gmail.com"
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
                      Create Account
                  </Button>

          <div className="auth-link">
            Already have an account?{" "}
            <Link to="/login">
              <span>Login</span>
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}