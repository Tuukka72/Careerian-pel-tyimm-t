import { Routes, Route } from "react-router-dom"
import { useEffect, useState } from "react"

import Mainsivu from "./Pages/Mainsivu"
import NotFound from "./Pages/NotFound"
import Uusikyyti from "./Pages/uusi_kyyti"
import Navbar from "./Pages/Navbar"
import PoistaKyydit from "./Pages/PoistaKyydit"

function App() {
  const [theme, setTheme] = useState("light")

  useEffect(() => {
    fetch("/api/test")
      .then(res => res.text())
      .then(data => console.log(data))
      .catch(err => console.error(err))
  }, [])

  useEffect(() => {
    const saved = localStorage.getItem("theme")

    if (saved) {
      setTheme(saved)
    } else if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
      setTheme("dark")
    }
  }, [])

  useEffect(() => {
    if (theme === "dark") {
      document.body.classList.add("dark-mode")
    } else {
      document.body.classList.remove("dark-mode")
    }

    localStorage.setItem("theme", theme)
  }, [theme])

  return (
    <>
      <Navbar theme={theme} setTheme={setTheme} />

      <Routes>
        <Route path="/" element={<Mainsivu />} />
        <Route path="/uusikyyti" element={<Uusikyyti />} />
        <Route path="/poista" element={<PoistaKyydit />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </>
  )
}

export default App