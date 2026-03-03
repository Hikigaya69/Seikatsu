import "./Hero.css";

function Hero() {
  return (
    <section className="hero-section">

      <div className="hero-bg-gradient"></div>

      <div className="hero-bg-image"></div>

      <div className="hero-content">

        <div className="hero-badge">
          <span className="badge-dot"></span>
          <span>Now delivering across Japan</span>
        </div>

        <h1>
          Your Starter
          <span className="highlight"> Supermarket </span>
          in Japan
        </h1>

        <p>
          Discover authentic international groceries, from Indian spices
          to Korean snacks, delivered right to your door anywhere in Japan.
        </p>

        <div className="hero-buttons">
          <button className="primary-btn">
            Explore Products
          </button>

          <button className="secondary-btn">
            Login
          </button>
        </div>

        <div className="hero-stats">
          <div>
            <h3>500+</h3>
            <span>Products</span>
          </div>

          <div>
            <h3>50+</h3>
            <span>Countries</span>
          </div>

          <div>
            <h3>24h</h3>
            <span>Delivery</span>
          </div>
        </div>

      </div>
    </section>
  );
}

export default Hero;