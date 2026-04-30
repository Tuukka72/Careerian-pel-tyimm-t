import { Routes, Route } from "react-router-dom"
import Mainsivu from "./Pages/Mainsivu"
import NotFound from "./Pages/NotFound"
import Uusikyyti from "./Pages/uusi_kyyti"
import Navbar from "./Pages/Navbar"
import PoistaKyydit from "./Pages/PoistaKyydit"

function App() {
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