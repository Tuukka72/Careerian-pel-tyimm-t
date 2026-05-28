import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import styles from "../css/mainsivu.module.css";

export default function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleRegister = async () => {
    if (!username || !password) {
      alert("Please fill all fields");
      return;
    }

    setLoading(true);

    try {
      const response = await fetch(
        "https://localhost:7150/login/register",
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
        const errorMessage = await response.text();
        throw new Error(errorMessage || "Registration failed");
      }

      alert("Registration successful!");
      navigate("/");
    } catch (error) {
      console.error("Register error:", error);
      alert("Could not register user. Check backend or connection.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <h1>Register</h1>

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
          className={styles["new-btn"]}
          onClick={handleRegister}
          disabled={loading}
        >
          {loading ? "Registering..." : "Register"}
        </button>

        <p>
          Already have an account? <Link to="/">Login</Link>
        </p>
      </div>
    </div>
  );
}