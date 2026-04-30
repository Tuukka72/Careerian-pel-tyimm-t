import Navbar from './Navbar';
import styles from '../Pages/mainsivu.module.css';

export default function App() {
  return (
    <>
      <Navbar />

      <div className={styles.container}>
        <div className={styles.card}>
          <button className={styles["new-btn"]}>Luo uusi</button>

          <div className={styles["list-box"]}>
            <div className={styles["list-header"]}>
              <span className={styles.collapse}>−</span>
              <h2>List</h2>
            </div>

            <div className={styles["list-content"]}>
              <p>tikkurila ---&gt; PMT</p>

              <div className={styles.date}>
                päivä: 28.4.2026
              </div>
            </div>
          </div>

        </div>
      </div>
    </>
  );
}