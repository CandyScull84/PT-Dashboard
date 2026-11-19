// src/components/Header.jsx
import React from "react";
// import logoUrl from '../assets/Logo.png' Logo removed

const Header = () => {
    return (
        <header className= "topbar">
            
            <div className="brand" >
                <span className="brand__logo">PT Dashboard</span>
            </div>
            <div className="container">
                <div className="topbar__inner">
                    <nav className="nav" aria-label="Huvudnavigation">
                        <a className="nav__link" href="#produkter">Produkter</a>
                        <a className="nav__link" href="#rapporter">Rapporter</a>
                        <a className="nav__link" href="#ordrar">Ordrar</a>
                        <a className="nav__link" href="#diagram">Diagram</a>
                    </nav>  
                </div>       
            </div>
        </header>
    );
}

export default Header;

