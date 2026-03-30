import { useState } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";

export default function Register() {
    const navigate = useNavigate();

  const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [email, setEmail] = useState("");
  const [error, setError] = useState("");

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      await axios.post(
          "/api/Auth/register",
          { FullName: username, Password: password, Email: email }
      );

      navigate("/login");
    } catch {
      setError("User already exists");
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-left">
        <h1>Create Account</h1>
        <p>
          Join Seikatsu and start exploring authentic
          international groceries across Japan.
        </p>
      </div>

      <div className="auth-right">
        <form className="auth-card" onSubmit={handleRegister}>
          <h2>Register</h2>

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
                      placeholder="email"
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

          <button type="submit">Create Account</button>

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