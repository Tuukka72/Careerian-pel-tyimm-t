import { useEffect, useState } from "react";
import styles from "../css/mainsivu.module.css";

export default function PoistaKyydit() {
  const [items, setItems] = useState([]);

  useEffect(() => {
    fetch("https://localhost:7150/kyydit")
      .then((res) => {
        if (!res.ok) {
          throw new Error(
            "Failed to fetch rides"
          );
        }

        return res.json();
      })
      .then((data) => setItems(data))
      .catch((err) =>
        console.error(err)
      );
  }, []);

  const deleteItem = async (id) => {
    try {
      const response = await fetch(
        `https://localhost:7150/kyydit/delete/${id}`,
        {
          method: "DELETE",
        }
      );

      if (!response.ok) {
        throw new Error(
          "Failed to delete ride"
        );
      }

      // Remove from frontend list
      setItems(
        items.filter(
          (item) => item.id !== id
        )
      );

    } catch (err) {
      console.error(err);

      alert("Kyydin poisto epäonnistui");
    }
  };

  return (
    <>
      <div className={styles.container}>
        <div className={styles.card}>
          <h2>Poista kyydit</h2>

          {items.length === 0 ? (
            <p>Ei kyytejä</p>
          ) : (
            items.map((item) => (
              <div
                key={item.id}
                className={
                  styles["list-content"]
                }
              >
                <p>
                  Kuski: {item.name}
                </p>

                <p>
                  Reitti:
                  {" "}
                  {item.mista}
                  {" → "}
                  {item.mihin}
                </p>

                <p>
                  Tyyppi:
                  {" "}
                  {item.tyyppi}
                </p>

                <p>
                  Lähtöaika:
                  {" "}
                  {new Date(
                    item.lahtoaika
                  ).toLocaleString()}
                </p>

                <p>
                  Paikkoja:
                  {" "}
                  {item.paikkoja}
                </p>

                <p>
                  Lisätiedot:
                  {" "}
                  {item.lisatiedot}
                </p>

                <button
                  onClick={() =>
                    deleteItem(item.id)
                  }
                >
                  Poista
                </button>
              </div>
            ))
          )}
        </div>
      </div>
    </>
  );
}