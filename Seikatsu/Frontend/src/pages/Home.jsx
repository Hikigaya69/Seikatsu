import NavbarHome from "../components/NavbarHome";
import FeaturedProducts from "../components/FeaturedProducts";
import Footer from "../components/Footer";

import { Container, Grid, Typography, Card, CardContent } from "@mui/material";

import "./Home.css";

export default function Home() {

  const categories = [
  { name: "Japanese Food", icon: "🍣" },
  { name: "Indian Groceries", icon: "🌶️" },
  { name: "Korean Snacks", icon: "🍜" },
  { name: "Drinks", icon: "🥤" },
  { name: "Instant Meals", icon: "🍱" },
  { name: "Daily Essentials", icon: "🛒" }
];

  return (
    <>
      <NavbarHome />

      {/* HERO */}

      <section className="hero-modern">
        <div className="hero-content-modern">
          <h1>International Groceries in Japan</h1>
          <p>Discover authentic foods from around the world.</p>
          <button className="hero-btn">Shop Now</button>
        </div>
      </section>

      {/* CATEGORY SECTION */}

      <Container sx={{ mt: 10 }}>

        <Typography variant="h4" sx={{ mb: 4, fontWeight: 700 }}>
          Shop by Category
        </Typography>

        <Grid container spacing={3}>

          {categories.map((category, index) => (

            <Grid item xs={12} sm={6} md={4} key={index}>

              <Card className="category-card">

                <CardContent>

                    <h2>{category.icon}</h2>

                        <Typography variant="h6">
                            {category.name}
                        </Typography>

                </CardContent>

              </Card>

            </Grid>

          ))}

        </Grid>

      </Container>

      {/* FEATURED PRODUCTS */}

      <FeaturedProducts />

      <Footer />
    </>
  );
}