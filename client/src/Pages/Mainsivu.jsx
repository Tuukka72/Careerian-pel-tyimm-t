import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import styles from "../css/mainsivu.module.css";

export default function Mainsivu() {
  const [items, setItems] = useState([]);

  useEffect(() => {
    fetch("https://localhost:7150/kyydit")
      .then(res => {
        if (!res.ok) {
          throw new Error("Failed to fetch rides");
        }
        return res.json();
      })
      .then(data => setItems(data))
      .catch(err => console.error(err));
  }, []);

  return (
    <>
      <div className={styles.container}>
        <div className={styles.card}>

          <Link to="/uusikyyti" className={styles["new-btn"]}>
            Uusi kyyti
          </Link>

          <div className={styles["list-box"]}>
            <div className={styles["list-header"]}>
              <span className={styles.collapse}>−</span>
              <h2>List</h2>
            </div>

            <div className={styles["list-content"]}>
              {items.map((item, i) => (
                <div key={item.id}>
                  <p>Kuski: {item.name}</p>
                  <p>Mistä-mihin: {item.mista} → {item.mihin}</p>
                  <p>Lähtöaika: {item.lahtoaika}</p>
                  <p>Paikkoja saatavilla: {item.paikkoja}</p>
                  <p>Tyyppi: {item.tyyppi}</p>
                  <p>Lisätiedot: {item.lisatiedot}</p>
                </div>
              ))}
            </div>
          </div>

        </div>
      </div>
    </>
  );
}