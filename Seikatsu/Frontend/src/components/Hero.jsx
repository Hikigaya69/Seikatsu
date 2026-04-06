import "./Hero.css";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
function Hero() {
  return (
    <section className="hero-section">

      <div className="hero-bg-gradient"></div>

      <div className="hero-bg-image"></div>

      <div className="hero-content">

        <div className="hero-badge">
                  <span className="badge-dot"></span>
                  <Badge variant="secondary bg-white-100 border-none text-lg">
                      Now delivering across 日本.
                  </Badge>
        
        </div>

        <h1>
          Your Starter
          <span className="highlight"> Supermarket </span>
          in Japan
        </h1>
        

        <p>
          Discover authentic international groceries, from Indian spices
          to Korean snacks, delivered right to your door anywhere in 日本.
        </p>

              <div className="hero-buttons">
         <Link to ="/home">
                  <Button variant="outline" className="px-10 py-6 text-lg rounded-xl border-gray-300 bg-rose-500
    shadow-[0_6px_0_rgba(0,0,0,0.2)]
    hover:shadow-[0_10px_20px_rgba(0,0,0,0.2)]
    hover:-translate-y-1
    active:translate-y-1 active:shadow-[0_2px_0_rgba(0,0,0,0.2)]
    transition-all duration-1500">
                      Explore Products
                  </Button>
                  </Link>
                  <Link to="/login">
                      <Button variant="outline" className="px-10 py-6 text-lg rounded-xl border-gray-300
    shadow-[0_6px_0_rgba(0,0,0,0.2)]
    hover:shadow-[0_10px_20px_rgba(0,0,0,0.2)]
    hover:-translate-y-1
    active:translate-y-1 active:shadow-[0_2px_0_rgba(0,0,0,0.2)]
    transition-all duration-1500">
                          Login
                      </Button>
                  </Link>


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