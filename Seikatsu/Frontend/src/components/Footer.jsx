import "./Footer.css";

function Footer() {
  return (
    <footer className="footer">

      <div className="footer-container">

        <div className="footer-left">
          <h3>Seikatsu</h3>
          <p>
            Your trusted international supermarket in Japan.
            We bring the world's finest groceries to your doorstep
            with quality, convenience, and care.
          </p>
        </div>

        <div className="footer-links">
          <h4>Quick Links</h4>
          <ul>
            <li>About Us</li>
            <li>Products</li>
            <li>Categories</li>
            <li>FAQ</li>
          </ul>
        </div>

        <div className="footer-contact">
          <h4>Contact</h4>
          <p>Tokyo, Japan</p>
          <p>+81 3-1234-5678</p>
          <p>hello@seikatsu.jp</p>
        </div>

      </div>

    </footer>
  );
}

export default Footer;