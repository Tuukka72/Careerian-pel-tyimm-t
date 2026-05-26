import { useState } from "react";
import style from "../css/uusikyyticss.module.css";

export default function App() {
  const [type, setType] = useState("");
  const [route, setRoute] = useState("");
  const [details, setDetails] = useState("");
  const [seatCount, setSeatCount] = useState("");
  const [time, setTime] = useState("");
  const [showPopup, setShowPopup] = useState(false);

  const loggedInUser = JSON.parse(
    localStorage.getItem("loggedInUser")
  );

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!loggedInUser) {
      alert("You must be logged in");
      return;
    }

    if (!route || !type || !seatCount || !time) {
      alert("Täytä kaikki kentät");
      return;
    }

    try {
      const parts = route.split(" → ");

      const mista = parts[0];
      const mihin = parts[1];

      const today = new Date()
        .toISOString()
        .split("T")[0];

      const response = await fetch(
        "https://localhost:7150/kyydit/add",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            kuski_Id: loggedInUser.id,
            mista: mista,
            mihin: mihin,
            lahtoaika: time,
            paikkoja: Number(seatCount),
            tyyppi: type,
            lisatiedot: details,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to add ride");
      }

      setShowPopup(true);

      setRoute("");
      setType("");
      setDetails("");
      setSeatCount("");
      setTime("");
    } catch (err) {
      console.error(err);

      alert("Kyydin lisääminen epäonnistui");
    }
  };

  return (
    <div className={style.container}>
      <div className={style.card}>
        <form onSubmit={handleSubmit}>

          <div className={style["top-row"]}>
            <select
              value={route}
              onChange={(e) => setRoute(e.target.value)}
              className={style["route-input"]}
            >
              <option value="">Valitse reitti</option>

              <option value="Tikkurila → PMT">
                Tikkurila → PMT
              </option>

              <option value="PMT → Tikkurila">
                PMT → Tikkurila
              </option>
            </select>

            <div className={style["button-group"]}>
              <button
                type="button"
                className={
                  type === "tarjoaa"
                    ? style.active
                    : ""
                }
                onClick={() => setType("tarjoaa")}
              >
                tarjoaa
              </button>

              <button
                type="button"
                className={
                  type === "pyytää"
                    ? style.active
                    : ""
                }
                onClick={() => setType("pyytää")}
              >
                pyytää
              </button>
            </div>
          </div>

          <div className={style.selection}>
            {type
              ? `Valittu: ${type}`
              : "nappien valinta"}
          </div>

          <textarea
            placeholder="lisätiedot tähän"
            value={details}
            onChange={(e) =>
              setDetails(e.target.value)
            }
            className={style.details}
          />

          <input
            type="number"
            placeholder="Paikkoja vapaana"
            value={seatCount}
            onChange={(e) =>
              setSeatCount(e.target.value)
            }
          />

          <input
            type="datetime-local"
            value={time}
            onChange={(e) =>
              setTime(e.target.value)
            }
          />

          <button
            type="submit"
            className={style["submit-btn"]}
          >
            Lähetä
          </button>

        </form>

        {showPopup && (
          <div className={style.popupOverlay}>
            <div className={style.popup}>
              <p>Kyytipyyntö lisätty!</p>

              <button
                onClick={() =>
                  setShowPopup(false)
                }
              >
                OK
              </button>
            </div>
          </div>
        )}

      </div>
    </div>
  );
}