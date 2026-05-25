import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import styles from "../css/mainsivu.module.css";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleLogin = async () => {
    if (!username || !password) {
      alert("Please fill all fields");
      return;
    }

    setLoading(true);

    try {
      const response = await fetch(
        "https://localhost:7150/login/check",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            name: username,
            password: password,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Wrong username or password");
      }

      const data = await response.json();

      // Save logged in user
      localStorage.setItem(
        "loggedInUser",
        JSON.stringify(data)
      );

      console.log("Logged in:", data);

      alert("Login successful!");

      navigate("/home");
    } catch (error) {
      console.error("Login error:", error);

      alert("Invalid username or password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <h1>Login</h1>

        <div className={styles.formGroup}>
          <input
            type="text"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            disabled={loading}
          />
        </div>

        <div className={styles.formGroup}>
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
          />
        </div>

        <button
          className={styles.newBtn}
          onClick={handleLogin}
          disabled={loading}
        >
          {loading ? "Logging in..." : "Login"}
        </button>

        <p>
          Don't have an account?{" "}
          <Link to="/register">Register</Link>
        </p>
      </div>
    </div>
  );
}