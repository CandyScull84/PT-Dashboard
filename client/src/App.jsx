import Header from "./components/Header";
import Footer from "./components/Footer";
import React from "react";
import OrdersSection from "./components/OrdersSection";
import ProductionSection from "./components/ProductionSection";
import EndOfDaySection from "./components/EndOfDaySection";
// import {endOfDay } from "./data/mock";

const App = () => {
  return (
    <>
      <Header />
      <main className="dashboard-grid">
        <OrdersSection/>
        <ProductionSection/>   
        <EndOfDaySection/>
      </main>
      <Footer />
    </>
  )
}
export default App;
