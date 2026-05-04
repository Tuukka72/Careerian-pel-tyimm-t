import { Routes, Route } from "react-router-dom"
import { useEffect } from "react"
import Mainsivu from "./Pages/Mainsivu"
import NotFound from "./Pages/NotFound"
import Uusikyyti from "./Pages/uusi_kyyti"
import Navbar from "./Pages/Navbar"
import PoistaKyydit from "./Pages/PoistaKyydit"

function App() {

  useEffect(() => {
    fetch("/api/test")
      .then(res => res.text())
      .then(data => console.log(data))
      .catch(err => console.error(err));
  }, []);

  return (
    <>
      <Navbar />

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