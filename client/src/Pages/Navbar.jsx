import styles from './Navbar.module.css';

export default function Navbar() {
  return (
    <nav className={styles.navbar}>
      <h1 className={styles.logo}>MyApp</h1>

      <div className={styles.links}>
        <a href="#">Home</a>
        <a href="#">Lists</a>
        <a href="#">About</a>
      </div>
    </nav>
  );
}