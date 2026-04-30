import { useState } from "react";
import style from "../css/uusikyyticss.module.css";

export default function App() {
  const [type, setType] = useState("");
  const [route, setRoute] = useState("");
  const [details, setDetails] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log({ route, type, details });
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
              <option value="Tikkurila → PMT">Tikkurila → PMT</option>
              <option value="PMT → Tikkurila">PMT → Tikkurila</option>
            </select>

            <div className={style["button-group"]}>
              <button
                type="button"
                className={type === "tarjoaa" ? style.active : ""}
                onClick={() => setType("tarjoaa")}
              >
                tarjoaa
              </button>

              <button
                type="button"
                className={type === "pyytää" ? style.active : ""}
                onClick={() => setType("pyytää")}
              >
                pyytää
              </button>
            </div>
          </div>

          <div className={style.selection}>
            {type ? `Valittu: ${type}` : "nappien valinta"}
          </div>

          <textarea
            placeholder="lisätiedot tähän"
            value={details}
            onChange={(e) => setDetails(e.target.value)}
            className={style.details}
          />

          <button type="submit" className={style["submit-btn"]}>
            Lähetä
          </button>

        </form>
      </div>
    </div>
  );
}