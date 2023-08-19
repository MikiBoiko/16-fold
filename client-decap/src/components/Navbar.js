import logo from "../imgs/logo_white.png"
import "./Navbar.css";

const Navbar = () => {
    return (
        <div className="Navbar">
            <div className="Navbar-title">
                <img src={ logo } alt="" />
                16fold.com
            </div>
            <div className="Navbar-links">
                <div className="Navbar-link">PLAY</div>
                <div className="Navbar-link">LEARN</div>
                <div className="Navbar-link">LOGIN</div>
                <div className="Navbar-link">REGISTER</div>
            </div>
        </div>
    );
};

export default Navbar;