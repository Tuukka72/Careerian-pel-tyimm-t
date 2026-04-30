import { useEffect, useState } from "react";
import styles from "../css/mainsivu.module.css";

export default function PoistaKyydit() {
  const [items, setItems] = useState([]);

  useEffect(() => {
    const data = JSON.parse(localStorage.getItem("list") || "[]");
    setItems(data);
  }, []);
const deleteItem = (indexToRemove) => {
  const updated = items.filter((_, index) => index !== indexToRemove);

  setItems(updated);
  localStorage.setItem("list", JSON.stringify(updated));
};

return (
  <>

    <div className={styles.container}>
      <div className={styles.card}>
        <h2>Poista kyydit</h2>

        {items.length === 0 ? (
          <p>Ei kyytejä</p>
        ) : (
          items.map((item, i) => (
            <div key={i} className={styles["list-content"]}>
              <p>{item.route}</p>
              <p>{item.type}</p>
              <p>{item.details}</p>

              <button onClick={() => deleteItem(i)}>
                Poista
              </button>
            </div>
          ))
        )}
      </div>
    </div>
  </>
);}