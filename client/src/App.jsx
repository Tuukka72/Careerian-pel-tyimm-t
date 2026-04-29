app.jsx
import { useState } from 'react'
import "./App.css";

export default function App() {
  return (
    <div className="container">
      <div className="card">
        
        <button className="new-btn">Luo uusi</button>

        <div className="list-box">
          <div className="list-header">
            <span className="collapse">−</span>
            <h2>List</h2>
          </div>

          <div className="list-content">
            <p>tikkurila ---&gt; PMT</p>

            <div className="date">
              päivä: 28.4.2026
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}