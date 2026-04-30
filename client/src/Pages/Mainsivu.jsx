import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Navbar from "./Navbar";
import styles from "../css/mainsivu.module.css";

export default function App() {
  const [items, setItems] = useState([]);

  useEffect(() => {
    const data = JSON.parse(localStorage.getItem("list") || "[]");
    setItems(data);
  }, []);

  return (
    <>
      <Navbar />

      <div className={styles.container}>
        <div className={styles.card}>
          <Link to="/uusikyyti" className={styles["new-btn"]}>Uusi kyyti</Link>
          <div className={styles["list-box"]}>
            <div className={styles["list-header"]}>
              <span className={styles.collapse}>−</span>
              <h2>List</h2>
            </div>

            <div className={styles["list-content"]}>
              {items.map((item, i) => (
                <div key={i}>
                  <p>{item.route}</p>
                  <p>{item.type}</p>
                  <p>{item.details}</p>
                </div>
              ))}
            </div>

          </div>
        </div>
      </div>
    </>
  );
}