import "./CountrySection.css";
import japanImg from "../assets/foodimages/japan.jpg";
import indiaImg from "../assets/foodimages/india.png";
import koreaImg from "../assets/foodimages/korea.jpg";
import usaImg from "../assets/foodimages/america.jpg";

const countries = [
  { name: "Japan", items: "120+ items", image: japanImg },
  { name: "India", items: "85+ items", image: indiaImg },
  { name: "Korea", items: "65+ items", image: koreaImg },
    { name: "USA", items: "50+ items", image: usaImg },
];

function CountrySection() {
  return (
      <section className="country-section  bg-gradient-to-r from-[#fce4ec] via-[#e8f5e9] via-[#fff3e0] to-[#e3f2fd]">
      
          <div className="country-container">

        <h2>Shop by Country</h2>
        <p>
          Explore our diverse selection of international groceries
        </p>
       

        <div className="country-grid">
          {countries.map((country, index) => (
            <div className="country-card" key={index}>
              <img src={country.image} alt={country.name} />
              <div className="overlay">
                <h3>{country.name}</h3>
                <span>{country.items}</span>
              </div>
            </div>
          ))}
        </div>

      </div>
    </section>
  );
}

export default CountrySection;