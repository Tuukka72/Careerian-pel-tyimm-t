import styles from "../css/Navbar.module.css";
import { Link } from "react-router-dom";

export default function Navbar({ theme, setTheme }) {
  return (
    <nav className={styles.navbar}>
      <h1 className={styles.logo}>MyApp</h1>

      <div className={styles.links}>
        <Link to="/">Home</Link>
        <Link to="/uusikyyti">Uusi kyyti</Link>
      </div>
    </nav>
  );
}