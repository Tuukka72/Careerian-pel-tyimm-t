import { Routes, Route } from "react-router-dom"
import { useEffect, useState } from "react"
//sivut
import Mainsivu from "./Pages/Mainsivu"
import NotFound from "./Pages/NotFound"
import Uusikyyti from "./Pages/uusi_kyyti"
import Navbar from "./Pages/Navbar"
import PoistaKyydit from "./Pages/PoistaKyydit"
import Login from "./Pages/login"
import Register from "./Pages/Register"

function App() {
  const [theme, setTheme] = useState("light")

  return (
    <>
      <Navbar theme={theme} setTheme={setTheme} />

      <Routes>
        <Route path="/" element={<Register />} />
        <Route path="/home" element={<Mainsivu />} />
        <Route path="/uusikyyti" element={<Uusikyyti />} />
        <Route path="/poista" element={<PoistaKyydit />} />
        <Route path="*" element={<NotFound />} />

        <Route path="/login" element={<Login />} />
      </Routes>
    </>
  )
}

export default App