import { Routes, Route } from "react-router-dom"
import Mainsivu from "./Pages/Mainsivu"
import NotFound from "./Pages/NotFound"

function App() {
  return (
    <Routes>
      <Route path="/" element={<Mainsivu />} />

      {/* 404 page */}
      <Route path="*" element={<NotFound />} />
    </Routes>
  )
}

export default App