import { useState } from "react";
import "./App.css";

export default function kysymykset() {

  const [type, setType] = useState("");
  const [route, setRoute] = useState("");
  const [details, setDetails] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log({ route, type, details });
  };

  return (
    <div className="container">
      <div className="card">

        <form onSubmit={handleSubmit}>

          <div className="top-row">
            <input
              type="text"
              placeholder="mistä minne"
              value={route}
              onChange={(e) => setRoute(e.target.value)}
              className="route-input"
            />

            <div className="button-group">
              <button
                type="button"
                className={type === "tarjoaa" ? "active" : ""}
                onClick={() => setType("tarjoaa")}
              >
                tarjoaa
              </button>

              <button
                type="button"
                className={type === "pyytää" ? "active" : ""}
                onClick={() => setType("pyytää")}
              >
                pyytää
              </button>
            </div>
          </div>

          <div className="selection">
            {type ? `Valittu: ${type}` : "nappien valinta"}
          </div>

          <textarea
            placeholder="lisätiedot tähän"
            value={details}
            onChange={(e) => setDetails(e.target.value)}
            className="details"
          />

          <button type="submit" className="submit-btn">
            Lähetä
          </button>

        </form>

      </div>
    </div>
  );
}